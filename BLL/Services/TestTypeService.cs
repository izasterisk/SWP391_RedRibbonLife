using AutoMapper;
using BLL.Interfaces;
using BLL.DTO.TestType;
using DAL.IRepository;
using DAL.Models;

namespace BLL.Services;

public class TestTypeService : ITestTypeService
{
    private readonly IMapper _mapper;
    private readonly ITestTypeRepository _testTypeRepository;

    public TestTypeService(IMapper mapper, ITestTypeRepository testTypeRepository)
    {
        _mapper = mapper;
        _testTypeRepository = testTypeRepository;
    }

    public async Task<TestTypeDTO> CreateTestTypeAsync(TestTypeCreateDTO dto)
    {
        ArgumentNullException.ThrowIfNull(dto, $"{nameof(dto)} is null");
        
        var testType = _mapper.Map<TestType>(dto);
        var createdTestType = await _testTypeRepository.CreateTestTypeWithTransactionAsync(testType);
        return _mapper.Map<TestTypeDTO>(createdTestType);
    }

    public async Task<TestTypeDTO> UpdateTestTypeAsync(TestTypeUpdateDTO dto)
    {
        ArgumentNullException.ThrowIfNull(dto, $"{nameof(dto)} is null");
        
        var testType = await _testTypeRepository.GetTestTypeForUpdateAsync(dto.TestTypeId);
        if (testType == null)
        {
            throw new Exception("TestType not found.");
        }
        
        _mapper.Map(dto, testType);
        var updatedTestType = await _testTypeRepository.UpdateTestTypeWithTransactionAsync(testType);
        return _mapper.Map<TestTypeDTO>(updatedTestType);
    }

    public async Task<List<TestTypeDTO>> GetAllTestTypeAsync()
    {
        var testTypes = await _testTypeRepository.GetAllTestTypesAsync();
        return _mapper.Map<List<TestTypeDTO>>(testTypes);
    }

    public async Task<TestTypeDTO> GetTestTypeByIdAsync(int id)
    {
        var testType = await _testTypeRepository.GetTestTypeByIdAsync(id, true);
        if (testType == null)
        {
            throw new Exception("TestType not found.");
        }
        return _mapper.Map<TestTypeDTO>(testType);
    }

    public async Task<bool> DeleteTestTypeByIdAsync(int id)
    {
        var testType = await _testTypeRepository.GetTestTypeForUpdateAsync(id);
        if (testType == null)
        {
            throw new Exception("TestType not found.");
        }
        return await _testTypeRepository.DeleteTestTypeWithTransactionAsync(testType);
    }
}