using DAL.Models;

namespace DAL.IRepository;

public interface ITestTypeRepository
{
    Task<TestType> CreateTestTypeWithTransactionAsync(TestType testType);
    Task<TestType> UpdateTestTypeWithTransactionAsync(TestType testType);
    Task<List<TestType>> GetAllTestTypesAsync();
    Task<TestType?> GetTestTypeByIdAsync(int id, bool useNoTracking = true);
    Task<TestType?> GetTestTypeForUpdateAsync(int id);
    Task<bool> DeleteTestTypeWithTransactionAsync(TestType testType);
}