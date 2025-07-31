using DAL.Models;

namespace DAL.IRepository
{
    public interface ICategoryRepository : IRepository<Category>
    {
        Task<Category> CreateCategoryWithTransactionAsync(Category category);
        Task<Category> UpdateCategoryWithTransactionAsync(Category category);
        Task<bool> DeleteCategoryWithTransactionAsync(Category category);
        Task<bool> IsCategoryExistAsync(int categoryId);
        Task<Category?> GetCategoryByIdAsync(int categoryId);
        Task<bool> IsCategoryNameExistsAsync(string categoryName);
    }
}
