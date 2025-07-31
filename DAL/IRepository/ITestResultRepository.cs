using DAL.Models;

namespace DAL.IRepository;

public interface ITestResultRepository
{
    Task<TestResult> CreateTestResultWithTransactionAsync(TestResult testResult, Appointment appointment);
    Task<TestResult> UpdateTestResultAsync(TestResult testResult);
    Task<List<TestResult>> GetAllTestResultsWithRelationsAsync();
    Task<TestResult?> GetTestResultWithRelationsAsync(int id);
    Task<List<TestResult>> GetTestResultsByDoctorIdWithRelationsAsync(int doctorId);
    Task<TestResult?> GetTestResultForUpdateAsync(int id);
    Task<bool> DeleteTestResultAsync(TestResult testResult);
}