using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
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

    }
}
