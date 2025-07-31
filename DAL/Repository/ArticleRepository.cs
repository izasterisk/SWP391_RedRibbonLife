using DAL.IRepository;
using DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace DAL.Repository;

public class ArticleRepository : IArticleRepository
{
    private readonly SWP391_RedRibbonLifeContext _dbContext;
    private readonly IRepository<Article> _articleRepository;
    
    public ArticleRepository(SWP391_RedRibbonLifeContext dbContext, IRepository<Article> articleRepository)
    {
        _dbContext = dbContext;
        _articleRepository = articleRepository;
    }
    
    public async Task<List<Article>> GetAllArticlesWithRelationsAsync()
    {
        return await _articleRepository.GetAllWithRelationsAsync(
            query => query.Include(a => a.Category)
                .Include(a => a.User)
        );
    }
    
    public async Task<Article?> GetArticleByIdWithRelationsAsync(int id, bool useNoTracking = true)
    {
        return await _articleRepository.GetWithRelationsAsync(
            a => a.ArticleId == id, 
            useNoTracking, 
            query => query.Include(a => a.Category)
                .Include(a => a.User)
        );
    }
    
    public async Task<Article> CreateArticleWithTransactionAsync(Article article)
    {
        await using var transaction = await _dbContext.Database.BeginTransactionAsync();
        try
        {
            article.IsActive = true;
            article.CreatedDate = DateOnly.FromDateTime(DateTime.Now);
            var createdArticle = await _articleRepository.CreateAsync(article);
            await transaction.CommitAsync();
            return createdArticle;
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
    
    public async Task<Article> UpdateArticleWithTransactionAsync(Article article)
    {
        await using var transaction = await _dbContext.Database.BeginTransactionAsync();
        try
        {
            var updatedArticle = await _articleRepository.UpdateAsync(article);
            await transaction.CommitAsync();
            return updatedArticle;
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
    
    public async Task<bool> DeleteArticleWithTransactionAsync(Article article)
    {
        await using var transaction = await _dbContext.Database.BeginTransactionAsync();
        try
        {
            var result = await _articleRepository.DeleteAsync(article);
            await transaction.CommitAsync();
            return result;
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
    
    public async Task<Article?> GetArticleForUpdateAsync(int id)
    {
        return await _articleRepository.GetAsync(u => u.ArticleId == id, useNoTracking: false);
    }
    
    public async Task<bool> CheckTitleExistsAsync(string title, int? excludeId = null)
    {
        if (excludeId.HasValue)
        {
            return await _articleRepository.AnyAsync(u => u.Title.Equals(title) && u.ArticleId != excludeId.Value);
        }
        return await _articleRepository.AnyAsync(u => u.Title.Equals(title));
    }
}