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
        private readonly IMapper _mapper;
        private readonly SWP391_RedRibbonLifeContext _dbContext;

        public ArticleService(IUserRepository<Article> articleRepository, IUserRepository<Category> categoryRepository, IMapper mapper, SWP391_RedRibbonLifeContext dbContext)
        {
            _articleRepository = articleRepository;
            _categoryRepository = categoryRepository;
            _mapper = mapper;
            _dbContext = dbContext;
        }
        public async Task<List<ArticleReadOnlyDTO>> GetAllArticleAsync()
        {
            // Sử dụng eager loading để lấy articles cùng với Category
            var articles = await _articleRepository.GetAllWithRelationsAsync(a => a.Category);
            if (articles == null || !articles.Any())
            {
                return new List<ArticleReadOnlyDTO>();
            }
            // Map articles sang ArticleDTO, AutoMapper sẽ tự động map CategoryName từ navigation property
            var articleDTOs = _mapper.Map<List<ArticleReadOnlyDTO>>(articles);
            return articleDTOs;
        }

        public async Task<bool> CreateArticleAsync(ArticleDTO dto)
        {
            ArgumentNullException.ThrowIfNull(dto, $"{nameof(dto)} is null");
            if (string.IsNullOrWhiteSpace(dto.Title))
                throw new ArgumentException("Title cannot be null or empty.", nameof(dto.Title));
            if (dto.Title.Length > 200)
                throw new ArgumentException("Title cannot exceed 200 characters.", nameof(dto.Title));
            if (string.IsNullOrWhiteSpace(dto.Content))
                throw new ArgumentException("Content cannot be null or empty.", nameof(dto.Content));
            // Begin transaction
            using var transaction = await _dbContext.Database.BeginTransactionAsync();
            try
            {
                //Check if title already exists
                var existingTitle = await _articleRepository.GetAsync(u => u.Title.Equals(dto.Title), true);
                if (existingTitle != null)
                {
                    throw new Exception("Title already exists.");
                }
                //Validate Category
                if(dto.CategoryId != null && dto.CategoryId >= 0)
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
                await _articleRepository.CreateAsync(article);
        
                // Commit transaction
                await transaction.CommitAsync();
                return true;
            }
            catch (Exception)
            {
                // Rollback transaction on error
                await transaction.RollbackAsync();
                throw; // Re-throw the exception
            }
        }
        
    }
}
