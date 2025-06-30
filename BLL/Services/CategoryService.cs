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
    
    public async Task<CategoryDTO> CreateCategoryAsync(CategoryDTO dto)
    {
        ArgumentNullException.ThrowIfNull(dto, $"{nameof(dto)} is null");
        using var transaction = await _dbContext.Database.BeginTransactionAsync();
        try
        {
            var category = _mapper.Map<Category>(dto);
            var createdCategory = await _categoryRepository.CreateAsync(category);
            await transaction.CommitAsync();
            return _mapper.Map<CategoryDTO>(createdCategory);
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
    
    public async Task<CategoryDTO> UpdateCategoryAsync(CategoryDTO dto)
    {
        ArgumentNullException.ThrowIfNull(dto, $"{nameof(dto)} is null");
        using var transaction = await _dbContext.Database.BeginTransactionAsync();
        try
        {
            var category = await _categoryRepository.GetAsync(u => u.CategoryId == dto.CategoryId, true);
            if (category == null)
            {
                throw new Exception("Category not found.");
            }
            _mapper.Map(dto, category);
            var updatedCategory = await _categoryRepository.UpdateAsync(category);
            await transaction.CommitAsync();
            return _mapper.Map<CategoryDTO>(updatedCategory);
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
    
    public async Task<List<CategoryDTO>> GetAllCategoryAsync()
    {
        var categories = await _categoryRepository.GetAllAsync();
        return _mapper.Map<List<CategoryDTO>>(categories);
    }
    
    public async Task<CategoryDTO> GetCategoryByIdAsync(int id)
    {
        var category = await _categoryRepository.GetAsync(u => u.CategoryId == id, true);
        if (category == null)
        {
            throw new Exception("Category not found.");
        }
        return _mapper.Map<CategoryDTO>(category);
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
