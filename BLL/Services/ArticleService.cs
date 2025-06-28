using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using BLL.DTO.Article;
using BLL.Interfaces;
using DAL.IRepository;
using DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace BLL.Services
{
    public class ArticleService : IArticleService
    {
        private readonly IUserRepository<Article> _articleRepository;
        private readonly IUserRepository<Category> _categoryRepository;
        private readonly IUserRepository<User> _userRepository;
        private readonly IMapper _mapper;
        private readonly SWP391_RedRibbonLifeContext _dbContext;

        public ArticleService(IUserRepository<Article> articleRepository, IUserRepository<Category> categoryRepository, IUserRepository<User> userRepository, IMapper mapper, SWP391_RedRibbonLifeContext dbContext)
        {
            _articleRepository = articleRepository;
            _categoryRepository = categoryRepository;
            _userRepository = userRepository;
            _mapper = mapper;
            _dbContext = dbContext;
        }
        public async Task<List<ArticleReadOnlyDTO>> GetAllArticleAsync()
        {
            var articles = await _articleRepository.GetAllWithRelationsAsync(
                query => query.Include(a => a.Category).Include(a => a.User)
            );
            if (articles == null || !articles.Any())
            {
                return new List<ArticleReadOnlyDTO>();
            }
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
            if (string.IsNullOrWhiteSpace(dto.Title))
                throw new ArgumentException("Title cannot be null or empty.", nameof(dto.Title));
            if (dto.Title.Length > 200)
                throw new ArgumentException("Title cannot exceed 200 characters.", nameof(dto.Title));
            if (string.IsNullOrWhiteSpace(dto.Content))
                throw new ArgumentException("Content cannot be null or empty.", nameof(dto.Content));
            if (dto.UserId <= 0)
                throw new ArgumentException("User ID must be a positive integer.", nameof(dto.UserId));
            // Begin transaction
            using var transaction = await _dbContext.Database.BeginTransactionAsync();
            try
            {
                var existingTitle = await _articleRepository.GetAsync(u => u.Title.Equals(dto.Title), true);
                if (existingTitle != null)
                {
                    throw new Exception("Title already exists.");
                }
                var existingUser = await _userRepository.GetAsync(u => u.UserId == dto.UserId, true);
                if (existingUser == null)
                {
                    throw new Exception("User not found.");
                }
                if(dto.CategoryId != null && dto.CategoryId > 0)
                {
                    var existingCategory = await _categoryRepository.GetAsync(u => u.CategoryId == dto.CategoryId, true);
                    if (existingCategory == null)
                    {
                        throw new Exception("Category not found.");
                    }
                }
                // Create article
                Article article = _mapper.Map<Article>(dto);
                article.IsActive = true; // Set default value for IsActive
                article.CreatedDate = DateOnly.FromDateTime(DateTime.Now); // Set current date
                var createdArticle = await _articleRepository.CreateAsync(article);
                // Load relations for response
                var articleWithRelations = await _articleRepository.GetWithRelationsAsync(
                    a => a.ArticleId == createdArticle.ArticleId, 
                    true, 
                    query => query.Include(a => a.Category).Include(a => a.User)
                );
                // Commit transaction
                await transaction.CommitAsync();
                return new
                {
                    ArticleInfo = _mapper.Map<ArticleReadOnlyDTO>(articleWithRelations)
                };
            }
            catch (Exception)
            {
                // Rollback transaction on error
                await transaction.RollbackAsync();
                throw; // Re-throw the exception
            }
        }
        public async Task<dynamic> UpdateArticleAsync(ArticleUpdateDTO dto)
        {
            ArgumentNullException.ThrowIfNull(dto, $"{nameof(dto)} is null");
            if (dto.Title != null && dto.Title.Length > 200)
                throw new ArgumentException("Title cannot exceed 200 characters.", nameof(dto.Title));
            if (dto.CategoryId != null && dto.CategoryId < 0)
                throw new ArgumentException("Category ID cannot be negative.", nameof(dto.CategoryId));
            if (dto.UserId != null && dto.UserId <= 0)
                throw new ArgumentException("User ID must be a positive integer.", nameof(dto.UserId));
            // Begin transaction
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
                //Validate User if being updated
                if (dto.UserId.HasValue)
                {
                    var existingUser = await _userRepository.GetAsync(u => u.UserId == dto.UserId.Value, true);
                    if (existingUser == null)
                    {
                        throw new Exception("User not found.");
                    }
                }
                //Validate Category if being updated
                if(dto.CategoryId.HasValue && dto.CategoryId > 0)
                {
                    var existingCategory = await _categoryRepository.GetAsync(u => u.CategoryId == dto.CategoryId, true);
                    if (existingCategory == null)
                    {
                        throw new Exception("Category not found.");
                    }
                }
                // Update article
                _mapper.Map(dto, article);
                var updatedArticle = await _articleRepository.UpdateAsync(article);
                // Load relations for response
                var articleWithRelations = await _articleRepository.GetWithRelationsAsync(
                    a => a.ArticleId == updatedArticle.ArticleId, 
                    true, 
                    query => query.Include(a => a.Category).Include(a => a.User)
                );
                // Commit transaction
                await transaction.CommitAsync();
                return new
                {
                    ArticleInfo = _mapper.Map<ArticleReadOnlyDTO>(articleWithRelations)
                };
            }
            catch (Exception)
            {
                // Rollback transaction on error
                await transaction.RollbackAsync();
                throw; // Re-throw the exception
            }
        }
        public async Task<bool> DeleteArticleByIdAsync(int id)
        {
            await using var transaction = await _dbContext.Database.BeginTransactionAsync();
            try
            {
                var article = await _articleRepository.GetAsync(u => u.ArticleId == id, true);
                if (article == null)
                {
                    throw new KeyNotFoundException($"Article with ID {id} not found");
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
}
