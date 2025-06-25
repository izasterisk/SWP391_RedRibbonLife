using BLL.DTO.TestResult;

namespace BLL.Interfaces;

public interface ITestResultService
{
    Task<TestResultDTO> CreateTestResultAsync(TestResultCreateDTO dto);
    Task<dynamic> UpdateTestResultAsync(TestResultUpdateDTO dto);
    Task<List<TestResultDTO>> GetAllTestResultAsync();
    Task<TestResultDTO> GetTestResultByIdAsync(int id);
    Task<bool> DeleteTestResultByIdAsync(int id);
}