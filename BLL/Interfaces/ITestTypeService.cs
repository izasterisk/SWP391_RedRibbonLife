using BLL.DTO.TestType;

namespace BLL.Interfaces;

public interface ITestTypeService
{
    Task<TestTypeDTO> CreateTestTypeAsync(TestTypeCreateDTO dto);
    Task<TestTypeDTO> UpdateTestTypeAsync(TestTypeUpdateDTO dto);
    Task<List<TestTypeDTO>> GetAllTestTypeAsync();
    Task<TestTypeDTO> GetTestTypeByIdAsync(int id);
    Task<bool> DeleteTestTypeAsync(int id);
}