using AutoMapper;
using BLL.DTO.Article;
using BLL.Interfaces;
using BLL.Utils;
using DAL.IRepository;
using DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace BLL.Services;

public class ArticleService : IArticleService
{
    private readonly IMapper _mapper;
    private readonly SWP391_RedRibbonLifeContext _dbContext;
    private readonly IUserRepository<Article> _articleRepository;
    private readonly IUserRepository<Category> _categoryRepository;
    private readonly IUserRepository<User> _userRepository;
    private readonly IUserUtils _userUtils;
    public ArticleService(IMapper mapper, SWP391_RedRibbonLifeContext dbContext, IUserRepository<Article> articleRepository, IUserRepository<Category> categoryRepository, IUserRepository<User> userRepository, IUserUtils userUtils)
    {
        _mapper = mapper;
        _dbContext = dbContext;
        _articleRepository = articleRepository;
        _categoryRepository = categoryRepository;
        _userRepository = userRepository;
        _userUtils = userUtils;
    }
    
    public async Task<List<ArticleReadOnlyDTO>> GetAllArticleAsync()
    {
        var articles = await _articleRepository.GetAllWithRelationsAsync(
            query => query.Include(a => a.Category).Include(a => a.User)
        );
        var articleDTOs = _mapper.Map<List<ArticleReadOnlyDTO>>(articles);
        return articleDTOs;
    }
    
    public async Task<ArticleReadOnlyDTO> GetArticleByIdAsync(int id)
    {
        var article = await _articleRepository.GetWithRelationsAsync(
            a => a.ArticleId == id, 
            true, 
            query => query.Include(a => a.Category).Include(a => a.User)
        );
        if (article == null)
        {
            throw new Exception($"Article with ID {id} not found");
        }
        return _mapper.Map<ArticleReadOnlyDTO>(article);
    }
    
    public async Task<dynamic> CreateArticleAsync(ArticleDTO dto)
    {
        ArgumentNullException.ThrowIfNull(dto, $"{nameof(dto)} is null");
        using var transaction = await _dbContext.Database.BeginTransactionAsync();
        try
        {
            var existingTitle = await _articleRepository.GetAsync(u => u.Title.Equals(dto.Title), true);
            if (existingTitle != null)
            {
                throw new Exception("Title already exists.");
            }
            dto.UserId.ValidateIfNotNull(_userUtils.CheckUserExist);
            if(dto.CategoryId != null)
            {
                var existingCategory = await _categoryRepository.GetAsync(u => u.CategoryId == dto.CategoryId, true);
                if (existingCategory == null)
                {
                    throw new Exception("Category not found.");
                }
            }
            Article article = _mapper.Map<Article>(dto);
            article.IsActive = true;
            article.CreatedDate = DateOnly.FromDateTime(DateTime.Now);
            var createdArticle = await _articleRepository.CreateAsync(article);
            var articleWithRelations = await _articleRepository.GetWithRelationsAsync(
                a => a.ArticleId == createdArticle.ArticleId, 
                true, 
                query => query.Include(a => a.Category).Include(a => a.User)
            );
            await transaction.CommitAsync();
            return new
            {
                ArticleInfo = _mapper.Map<ArticleReadOnlyDTO>(articleWithRelations)
            };
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
    
    public async Task<dynamic> UpdateArticleAsync(ArticleUpdateDTO dto)
    {
        ArgumentNullException.ThrowIfNull(dto, $"{nameof(dto)} is null");
        using var transaction = await _dbContext.Database.BeginTransactionAsync();
        try
        {
            var article = await _articleRepository.GetAsync(u => u.ArticleId == dto.ArticleId, true);
            if (article == null)
            {
                throw new Exception("No article found.");
            }
            
            if (!string.IsNullOrWhiteSpace(dto.Title))
            {
                var articleTitle = await _articleRepository.GetAsync(u => u.Title.Equals(dto.Title) && u.ArticleId != dto.ArticleId, true);
                if (articleTitle != null)
                {
                    throw new Exception("Title already exists.");
                }
            }
            dto.UserId.ValidateIfNotNull(_userUtils.CheckUserExist);
            if(dto.CategoryId.HasValue)
            {
                var existingCategory = await _categoryRepository.GetAsync(u => u.CategoryId == dto.CategoryId, true);
                if (existingCategory == null)
                {
                    throw new Exception("Category not found.");
                }
            }
            _mapper.Map(dto, article);
            var updatedArticle = await _articleRepository.UpdateAsync(article);
            var articleWithRelations = await _articleRepository.GetWithRelationsAsync(
                a => a.ArticleId == updatedArticle.ArticleId, 
                true, 
                query => query.Include(a => a.Category).Include(a => a.User)
            );
            await transaction.CommitAsync();
            return new
            {
                ArticleInfo = _mapper.Map<ArticleReadOnlyDTO>(articleWithRelations)
            };
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
    
    public async Task<bool> DeleteArticleByIdAsync(int id)
    {
        using var transaction = await _dbContext.Database.BeginTransactionAsync();
        try
        {
            var article = await _articleRepository.GetAsync(u => u.ArticleId == id, true);
            if (article == null)
            {
                throw new Exception($"Article with ID {id} not found");
            }
            await _articleRepository.DeleteAsync(article);
            await transaction.CommitAsync();
            return true;
        }
        catch(Exception)
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
}
