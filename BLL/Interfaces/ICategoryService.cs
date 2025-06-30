using BLL.DTO.Category;

namespace BLL.Interfaces;

public interface ICategoryService
{
    Task<CategoryDTO> CreateCategoryAsync(CategoryDTO dto);
    Task<CategoryDTO> UpdateCategoryAsync(CategoryDTO dto);
    Task<List<CategoryDTO>> GetAllCategoryAsync();
    Task<CategoryDTO> GetCategoryByIdAsync(int id);
    Task<bool> DeleteCategoryByIdAsync(int id);
}
