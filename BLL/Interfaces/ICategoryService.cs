using BLL.DTO.Category;

namespace BLL.Interfaces;

public interface ICategoryService
{
    Task<CategoryReadonlyDTO> CreateCategoryAsync(CategoryCreateDTO updateDto);
    Task<CategoryReadonlyDTO> UpdateCategoryAsync(CategoryUpdateDTO updateDto);
    Task<List<CategoryReadonlyDTO>> GetAllCategoryAsync();
    Task<CategoryReadonlyDTO> GetCategoryByIdAsync(int id);
    Task<bool> DeleteCategoryByIdAsync(int id);
}
