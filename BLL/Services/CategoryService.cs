using AutoMapper;
using BLL.DTO.Category;
using BLL.Interfaces;
using DAL.IRepository;
using DAL.Models;

namespace BLL.Services;

public class CategoryService : ICategoryService
{
    private readonly IMapper _mapper;
    private readonly ICategoryRepository _categoryRepository;

    public CategoryService(IMapper mapper, ICategoryRepository categoryRepository)
    {
        _mapper = mapper;
        _categoryRepository = categoryRepository;
    }
    
    public async Task<CategoryReadonlyDTO> CreateCategoryAsync(CategoryCreateDTO createDto)
    {
        ArgumentNullException.ThrowIfNull(createDto, $"{nameof(createDto)} is null");
        if (await _categoryRepository.IsCategoryNameExistsAsync(createDto.CategoryName))
        {
            throw new InvalidOperationException("Category name already exists.");
        }
        var category = _mapper.Map<Category>(createDto);
        var createdCategory = await _categoryRepository.CreateCategoryWithTransactionAsync(category);
        return _mapper.Map<CategoryReadonlyDTO>(createdCategory);
    }
    
    public async Task<CategoryReadonlyDTO> UpdateCategoryAsync(CategoryUpdateDTO updateDto)
    {
        ArgumentNullException.ThrowIfNull(updateDto, $"{nameof(updateDto)} is null");
        var category = await _categoryRepository.GetCategoryByIdAsync(updateDto.CategoryId);
        if (category == null)
        {
            throw new Exception("Category not found.");
        }
        if (await _categoryRepository.IsCategoryNameExistsAsync(updateDto.CategoryName))
        {
            throw new InvalidOperationException("Category name already exists.");
        }
        _mapper.Map(updateDto, category);
        var updatedCategory = await _categoryRepository.UpdateCategoryWithTransactionAsync(category);
        return _mapper.Map<CategoryReadonlyDTO>(updatedCategory);
    }
    
    public async Task<List<CategoryReadonlyDTO>> GetAllCategoryAsync()
    {
        var categories = await _categoryRepository.GetAllAsync();
        return _mapper.Map<List<CategoryReadonlyDTO>>(categories);
    }
    
    public async Task<CategoryReadonlyDTO> GetCategoryByIdAsync(int id)
    {
        if (id <= 0)
        {
            throw new ArgumentException("Category ID must be greater than 0.", nameof(id));
        }
        var category = await _categoryRepository.GetCategoryByIdAsync(id);
        if (category == null)
        {
            throw new Exception("Category not found.");
        }
        return _mapper.Map<CategoryReadonlyDTO>(category);
    }
    
    public async Task<bool> DeleteCategoryByIdAsync(int id)
    {
        if (id <= 0)
        {
            throw new ArgumentException("Category ID must be greater than 0.", nameof(id));
        }
        var category = await _categoryRepository.GetCategoryByIdAsync(id);
        if (category == null)
        {
            throw new Exception("Category not found.");
        }
        return await _categoryRepository.DeleteCategoryWithTransactionAsync(category);
    }
}
