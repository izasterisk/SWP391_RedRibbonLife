using DAL.IRepository;
using DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace DAL.Repository
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly SWP391_RedRibbonLifeContext _dbContext;
        private readonly IRepository<Category> _categoryRepository;
        public CategoryRepository(SWP391_RedRibbonLifeContext dbContext, IRepository<Category> categoryRepository)
        {
            _dbContext = dbContext;
            _categoryRepository = categoryRepository;
        }

        public async Task<Category> CreateCategoryWithTransactionAsync(Category category)
        {
            await using var transaction = await _dbContext.Database.BeginTransactionAsync();
            try
            {
                category.IsActive = true;
                var createdCategory = await _categoryRepository.CreateAsync(category);
                await transaction.CommitAsync();
                return createdCategory;
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<Category> UpdateCategoryWithTransactionAsync(Category category)
        {
            await using var transaction = await _dbContext.Database.BeginTransactionAsync();
            try
            {
                var updatedCategory = await _categoryRepository.UpdateAsync(category);
                await transaction.CommitAsync();
                return updatedCategory;
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<bool> DeleteCategoryWithTransactionAsync(Category category)
        {
            await using var transaction = await _dbContext.Database.BeginTransactionAsync();
            try
            {
                var result = await _categoryRepository.DeleteAsync(category);
                await transaction.CommitAsync();
                return result;
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<Category?> GetCategoryByIdAsync(int categoryId)
        {
            return await _categoryRepository.GetAsync(c => c.CategoryId == categoryId, useNoTracking: true);
        }
        
        public async Task<bool> IsCategoryNameExistsAsync(string categoryName)
        {
            return await _categoryRepository.AnyAsync(c => c.CategoryName == categoryName);
        }
        
        public async Task<List<Category>> GetAllAsync()
        {
            return await _categoryRepository.GetAllAsync();
        }
    }
}
