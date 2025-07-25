using BLL.DTO.TestResult;

namespace BLL.Interfaces;

public interface ITestResultService
{
    Task<TestResultDTO> CreateTestResultAsync(TestResultCreateDTO dto);
    Task<TestResultDTO> UpdateTestResultAsync(TestResultUpdateDTO dto);
    Task<List<TestResultDTO>> GetAllTestResultAsync();
    Task<TestResultDTO> GetTestResultByIdAsync(int id);
    Task<List<TestResultDTO>> GetTestResultByDoctorIdAsync(int doctorId);
    Task<bool> DeleteTestResultByIdAsync(int id);
}