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

namespace BLL.Services
{
    public class ArticleService : IArticleService
    {
        private readonly IUserRepository<Article> _articleRepository;
        private readonly IUserRepository<Category> _categoryRepository;
        private readonly IMapper _mapper;

        public ArticleService(IUserRepository<Article> articleRepository, IUserRepository<Category> categoryRepository, IMapper mapper)
        {
            _articleRepository = articleRepository;
            _categoryRepository = categoryRepository;
            _mapper = mapper;
        }
        public async Task<List<Article>> GetAllArticlesAsync()
        {
            return await _articleRepository.GetAllAsync();
        }
        public async Task<bool> CreateArticleAsync(ArticleDTO dto)
        {
            ArgumentNullException.ThrowIfNull(dto, $"{nameof(dto)} is null");
            if (string.IsNullOrWhiteSpace(dto.Title))
                throw new ArgumentNullException(nameof(dto.Title), "Title is required.");
            if (string.IsNullOrWhiteSpace(dto.Content))
                throw new ArgumentNullException(nameof(dto.Content), "Content is required.");
            //Check if Title already exists
            var existingTitle = await _articleRepository.GetAsync(u => u.Title.Equals(dto.Title));
            if (existingTitle != null)
            {
                throw new Exception("Title already exists.");
            }
            //Validate Category
            var validateCategory = await _categoryRepository.GetAsync(u => u.CategoryId == dto.CategoryId);
            if (validateCategory == null)
            {
                throw new ArgumentException("Invalid Category ID.", nameof(dto.CategoryId));
            }
            Article article = _mapper.Map<Article>(dto);
            article.IsActive = true; // Set default value for IsActive
            await _articleRepository.CreateAsync(article);
            return true;
        }

    }
}
