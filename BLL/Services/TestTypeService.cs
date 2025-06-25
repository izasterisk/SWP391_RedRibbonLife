using AutoMapper;
using BLL.Interfaces;
using BLL.DTO.TestType;
using DAL.IRepository;
using DAL.Models;

namespace BLL.Services;

public class TestTypeService : ITestTypeService
{
    private readonly IUserRepository<TestType> _testTypeRepository;
    private readonly IMapper _mapper;

    public TestTypeService(IUserRepository<TestType> testTypeRepository, IMapper mapper)
    {
        _testTypeRepository = testTypeRepository;
        _mapper = mapper;
    }

    public async Task<TestTypeDTO> CreateTestTypeAsync(TestTypeCreateDTO dto)
    {
        var testType = _mapper.Map<TestType>(dto);
        await _testTypeRepository.CreateAsync(testType);
        return _mapper.Map<TestTypeDTO>(testType);
    }

    public async Task<TestTypeDTO> UpdateTestTypeAsync(TestTypeUpdateDTO dto)
    {
        var testType = await _testTypeRepository.GetAsync(u => u.TestTypeId == dto.TestTypeId, true);
        if (testType == null)
        {
            throw new Exception($"TestType with ID {dto.TestTypeId} not found");
        }
        _mapper.Map(dto, testType);
        await _testTypeRepository.UpdateAsync(testType);
        return _mapper.Map<TestTypeDTO>(testType);
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
            throw new Exception($"TestType with ID {id} not found");
        }
        return _mapper.Map<TestTypeDTO>(testType);
    }

    public async Task<bool> DeleteTestTypeAsync(int id)
    {
        var testType = await _testTypeRepository.GetAsync(u => u.TestTypeId == id, true);
        if (testType == null)
        {
            throw new Exception($"TestType with ID {id} not found");
        }
        await _testTypeRepository.DeleteAsync(testType);
        return true;
    }
}