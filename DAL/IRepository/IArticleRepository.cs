using DAL.Models;

namespace DAL.IRepository;

public interface IArticleRepository
{
    Task<List<Article>> GetAllArticlesWithRelationsAsync();
    Task<Article?> GetArticleByIdWithRelationsAsync(int id, bool useNoTracking = true);
    Task<Article> CreateArticleWithTransactionAsync(Article article);
    Task<Article> UpdateArticleWithTransactionAsync(Article article);
    Task<bool> DeleteArticleWithTransactionAsync(Article article);
    Task<Article?> GetArticleForUpdateAsync(int id);
    Task<bool> CheckTitleExistsAsync(string title, int? excludeId = null);
}