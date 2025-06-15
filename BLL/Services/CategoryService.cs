using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using BLL.Interfaces;
using DAL.IRepository;
using DAL.Models;
using BLL.DTO.Category;

namespace BLL.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly IUserRepository<Category> _categoryRepository;
        private readonly IMapper _mapper;

        public CategoryService(IUserRepository<Category> categoryRepository, IMapper mapper)
        {
            _categoryRepository = categoryRepository;
            _mapper = mapper;
        }
        public async Task<CategoryDTO> CreateCategoryAsync(CategoryDTO dto)
        {
            var category = _mapper.Map<Category>(dto);
            await _categoryRepository.CreateAsync(category);
            return _mapper.Map<CategoryDTO>(category);
        }
        public async Task<CategoryDTO> UpdateCategoryAsync(CategoryDTO dto)
        {
            var category = await _categoryRepository.GetAsync(u => u.CategoryId == dto.CategoryId, true);
            if (category == null)
            {
                throw new Exception($"Category with ID {dto.CategoryId} not found");
            }
            _mapper.Map(dto, category);
            await _categoryRepository.UpdateAsync(category);
            return _mapper.Map<CategoryDTO>(category);
        }
        public async Task<List<CategoryDTO>> GetAllCategoryAsync()
        {
            var category = await _categoryRepository.GetAllAsync();
            return _mapper.Map<List<CategoryDTO>>(category);
        }
        public async Task<CategoryDTO> GetCategoryByIdAsync(int id)
        {
            var category = await _categoryRepository.GetAsync(u => u.CategoryId == id, true);
            if (category == null)
            {
                throw new Exception($"Category with ID {id} not found");
            }
            return _mapper.Map<CategoryDTO>(category);
        }
        public async Task<bool> DeleteCategoryByIdAsync(int id)
        {
            var category = await _categoryRepository.GetAsync(u => u.CategoryId == id, true);
            if (category == null)
            {
                throw new Exception($"Category with ID {id} not found");
            }
            await _categoryRepository.DeleteAsync(category);
            return true;
        }
    }
}
