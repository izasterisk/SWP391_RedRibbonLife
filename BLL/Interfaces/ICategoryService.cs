using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BLL.DTO.Category;

namespace BLL.Interfaces
{
    public interface ICategoryService
    {
        Task<CategoryDTO> CreateCategoryAsync(CategoryDTO dto);
        Task<List<CategoryDTO>> GetAllCategoryAsync();
        Task<CategoryDTO> GetCategoryByIdAsync(int id);
        Task<CategoryDTO> UpdateCategoryAsync(CategoryDTO dto);
        Task<bool> DeleteCategoryByIdAsync(int id);
    }
}
