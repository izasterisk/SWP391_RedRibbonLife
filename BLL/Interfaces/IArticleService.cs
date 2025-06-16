using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BLL.DTO.Article;

namespace BLL.Interfaces
{
    public interface IArticleService
    {
        Task<List<ArticleReadOnlyDTO>> GetAllArticleAsync();
        Task<bool> CreateArticleAsync(ArticleDTO dto);
        Task<bool> UpdateArticleAsync(ArticleUpdateDTO dto);
        Task<bool> DeleteArticleByIdAsync(int id);
        Task<ArticleReadOnlyDTO> GetArticleByIdAsync(int id);
    }
}
