using AutoMapper;
using BLL.Interfaces;
using BLL.DTO.TestType;
using DAL.IRepository;
using DAL.Models;

namespace BLL.Services;

public class TestTypeService : ITestTypeService
{
    private readonly IMapper _mapper;
    private readonly SWP391_RedRibbonLifeContext _dbContext;
    private readonly IUserRepository<TestType> _testTypeRepository;

    public TestTypeService(IMapper mapper, SWP391_RedRibbonLifeContext dbContext, IUserRepository<TestType> testTypeRepository)
    {
        _mapper = mapper;
        _dbContext = dbContext;
        _testTypeRepository = testTypeRepository;
    }

    public async Task<TestTypeDTO> CreateTestTypeAsync(TestTypeCreateDTO dto)
    {
        ArgumentNullException.ThrowIfNull(dto, $"{nameof(dto)} is null");
        using var transaction = await _dbContext.Database.BeginTransactionAsync();
        try
        {
            var testType = _mapper.Map<TestType>(dto);
            var createdTestType = await _testTypeRepository.CreateAsync(testType);
            await transaction.CommitAsync();
            return _mapper.Map<TestTypeDTO>(createdTestType);
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<TestTypeDTO> UpdateTestTypeAsync(TestTypeUpdateDTO dto)
    {
        ArgumentNullException.ThrowIfNull(dto, $"{nameof(dto)} is null");
        using var transaction = await _dbContext.Database.BeginTransactionAsync();
        try
        {
            var testType = await _testTypeRepository.GetAsync(u => u.TestTypeId == dto.TestTypeId, true);
            if (testType == null)
            {
                throw new Exception("TestType not found.");
            }
            _mapper.Map(dto, testType);
            var updatedTestType = await _testTypeRepository.UpdateAsync(testType);
            await transaction.CommitAsync();
            return _mapper.Map<TestTypeDTO>(updatedTestType);
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<List<TestTypeDTO>> GetAllTestTypeAsync()
    {
        var testTypes = await _testTypeRepository.GetAllAsync();
        return _mapper.Map<List<TestTypeDTO>>(testTypes);
    }

    public async Task<TestTypeDTO> GetTestTypeByIdAsync(int id)
    {
        var testType = await _testTypeRepository.GetAsync(u => u.TestTypeId == id, true);
        if (testType == null)
        {
            throw new Exception("TestType not found.");
        }
        return _mapper.Map<TestTypeDTO>(testType);
    }

    public async Task<bool> DeleteTestTypeByIdAsync(int id)
    {
        var testType = await _testTypeRepository.GetAsync(u => u.TestTypeId == id, true);
        if (testType == null)
        {
            throw new Exception("TestType not found.");
        }
        await _testTypeRepository.DeleteAsync(testType);
        return true;
    }
}