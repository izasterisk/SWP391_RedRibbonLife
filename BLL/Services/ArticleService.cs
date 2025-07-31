using AutoMapper;
using BLL.DTO.Article;
using BLL.Interfaces;
using BLL.Utils;
using DAL.IRepository;
using DAL.Models;

namespace BLL.Services;

public class ArticleService : IArticleService
{
    private readonly IMapper _mapper;
    private readonly IArticleRepository _articleRepository;
    private readonly IUserUtils _userUtils;
    
    public ArticleService(IMapper mapper, IArticleRepository articleRepository, IUserUtils userUtils)
    {
        _mapper = mapper;
        _articleRepository = articleRepository;
        _userUtils = userUtils;
    }
    
    public async Task<List<ArticleReadOnlyDTO>> GetAllArticleAsync()
    {
        var articles = await _articleRepository.GetAllArticlesWithRelationsAsync();
        var articleDTOs = _mapper.Map<List<ArticleReadOnlyDTO>>(articles);
        return articleDTOs;
    }
    
    public async Task<ArticleReadOnlyDTO> GetArticleByIdAsync(int id)
    {
        var article = await _articleRepository.GetArticleByIdWithRelationsAsync(id, true);
        if (article == null)
        {
            throw new Exception($"Article with ID {id} not found");
        }
        return _mapper.Map<ArticleReadOnlyDTO>(article);
    }
    
    public async Task<ArticleReadOnlyDTO> CreateArticleAsync(ArticleDTO dto)
    {
        ArgumentNullException.ThrowIfNull(dto, $"{nameof(dto)} is null");
        await dto.CategoryId.ValidateIfNotNullAsync(_userUtils.CheckCategoryExistAsync);
        await dto.UserId.ValidateIfNotNullAsync(_userUtils.CheckUserExistAsync);
        
        var existingTitleExists = await _articleRepository.CheckTitleExistsAsync(dto.Title);
        if (existingTitleExists)
        {
            throw new Exception("Title already exists.");
        }

        Article article = _mapper.Map<Article>(dto);
        var createdArticle = await _articleRepository.CreateArticleWithTransactionAsync(article);
        var articleWithRelations = await _articleRepository.GetArticleByIdWithRelationsAsync(createdArticle.ArticleId, true);
        return _mapper.Map<ArticleReadOnlyDTO>(articleWithRelations);
    }
    
    public async Task<ArticleReadOnlyDTO> UpdateArticleAsync(ArticleUpdateDTO dto)
    {
        ArgumentNullException.ThrowIfNull(dto, $"{nameof(dto)} is null");
        await dto.CategoryId.ValidateIfNotNullAsync(_userUtils.CheckCategoryExistAsync);
        await dto.UserId.ValidateIfNotNullAsync(_userUtils.CheckUserExistAsync);
        
        if (!string.IsNullOrWhiteSpace(dto.Title))
        {
            var titleExists = await _articleRepository.CheckTitleExistsAsync(dto.Title, dto.ArticleId);
            if (titleExists)
            {
                throw new Exception("Title already exists.");
            }
        }

        var article = await _articleRepository.GetArticleForUpdateAsync(dto.ArticleId);
        if (article == null)
        {
            throw new Exception("No article found.");
        }
        
        _mapper.Map(dto, article);
        var updatedArticle = await _articleRepository.UpdateArticleWithTransactionAsync(article);
        var articleWithRelations = await _articleRepository.GetArticleByIdWithRelationsAsync(updatedArticle.ArticleId, true);
        return _mapper.Map<ArticleReadOnlyDTO>(articleWithRelations);
    }
    
    public async Task<bool> DeleteArticleByIdAsync(int id)
    {
        var article = await _articleRepository.GetArticleForUpdateAsync(id);
        if (article == null)
        {
            throw new Exception($"Article with ID {id} not found");
        }
        
        return await _articleRepository.DeleteArticleWithTransactionAsync(article);
    }
}
