using DAL.Models;

namespace DAL.IRepository
{
    public interface ICategoryRepository
    {
        Task<Category> CreateCategoryWithTransactionAsync(Category category);
        Task<Category> UpdateCategoryWithTransactionAsync(Category category);
        Task<bool> DeleteCategoryWithTransactionAsync(Category category);
        Task<Category?> GetCategoryByIdAsync(int categoryId);
        Task<bool> IsCategoryNameExistsAsync(string categoryName);
        Task<List<Category>> GetAllAsync();
    }
}
