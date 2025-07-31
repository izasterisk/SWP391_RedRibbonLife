using DAL.IRepository;
using DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace DAL.Repository
{
    public class CategoryRepository : Repository<Category>, ICategoryRepository
    {
        private readonly SWP391_RedRibbonLifeContext _dbContext;

        public CategoryRepository(SWP391_RedRibbonLifeContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Category> CreateCategoryWithTransactionAsync(Category category)
        {
            await using var transaction = await _dbContext.Database.BeginTransactionAsync();
            try
            {
                category.IsActive = true;
                var createdCategory = await CreateAsync(category);
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
                var updatedCategory = await UpdateAsync(category);
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
                var result = await DeleteAsync(category);
                await transaction.CommitAsync();
                return result;
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<bool> IsCategoryExistAsync(int categoryId)
        {
            return await AnyAsync(c => c.CategoryId == categoryId);
        }

        public async Task<Category?> GetCategoryByIdAsync(int categoryId)
        {
            return await GetAsync(c => c.CategoryId == categoryId, useNoTracking: true);
        }
        
        public async Task<bool> IsCategoryNameExistsAsync(string categoryName)
        {
            return await AnyAsync(c => c.CategoryName == categoryName);
        }
    }
}
