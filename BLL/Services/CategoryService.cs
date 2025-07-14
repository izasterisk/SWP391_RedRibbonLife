using AutoMapper;
using BLL.DTO.Category;
using BLL.Interfaces;
using DAL.IRepository;
using DAL.Models;

namespace BLL.Services;

public class CategoryService : ICategoryService
{
    private readonly IMapper _mapper;
    private readonly SWP391_RedRibbonLifeContext _dbContext;
    private readonly IUserRepository<Category> _categoryRepository;

    public CategoryService(IMapper mapper, SWP391_RedRibbonLifeContext dbContext, IUserRepository<Category> categoryRepository)
    {
        _mapper = mapper;
        _dbContext = dbContext;
        _categoryRepository = categoryRepository;
    }
    
    public async Task<CategoryReadonlyDTO> CreateCategoryAsync(CategoryCreateDTO updateDto)
    {
        ArgumentNullException.ThrowIfNull(updateDto, $"{nameof(updateDto)} is null");
        await using var transaction = await _dbContext.Database.BeginTransactionAsync();
        try
        {
            var category = _mapper.Map<Category>(updateDto);
            category.IsActive = true;
            var createdCategory = await _categoryRepository.CreateAsync(category);
            await transaction.CommitAsync();
            return _mapper.Map<CategoryReadonlyDTO>(createdCategory);
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
    
    public async Task<CategoryReadonlyDTO> UpdateCategoryAsync(CategoryUpdateDTO updateDto)
    {
        ArgumentNullException.ThrowIfNull(updateDto, $"{nameof(updateDto)} is null");
        var category = await _categoryRepository.GetAsync(u => u.CategoryId == updateDto.CategoryId, true);
        if (category == null)
        {
            throw new Exception("Category not found.");
        }
        await using var transaction = await _dbContext.Database.BeginTransactionAsync();
        try
        {
            _mapper.Map(updateDto, category);
            var updatedCategory = await _categoryRepository.UpdateAsync(category);
            await transaction.CommitAsync();
            return _mapper.Map<CategoryReadonlyDTO>(updatedCategory);
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
    
    public async Task<List<CategoryReadonlyDTO>> GetAllCategoryAsync()
    {
        var categories = await _categoryRepository.GetAllAsync();
        return _mapper.Map<List<CategoryReadonlyDTO>>(categories);
    }
    
    public async Task<CategoryReadonlyDTO> GetCategoryByIdAsync(int id)
    {
        var category = await _categoryRepository.GetAsync(u => u.CategoryId == id, true);
        if (category == null)
        {
            throw new Exception("Category not found.");
        }
        return _mapper.Map<CategoryReadonlyDTO>(category);
    }
    
    public async Task<bool> DeleteCategoryByIdAsync(int id)
    {
        var category = await _categoryRepository.GetAsync(u => u.CategoryId == id, true);
        if (category == null)
        {
            throw new Exception("Category not found.");
        }
        await _categoryRepository.DeleteAsync(category);
        return true;
    }
}
