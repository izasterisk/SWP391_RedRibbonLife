using DAL.IRepository;
using DAL.Models;

namespace DAL.Repository;

public class TestTypeRepository : ITestTypeRepository
{
    private readonly SWP391_RedRibbonLifeContext _dbContext;
    private readonly IRepository<TestType> _testTypeRepository;

    public TestTypeRepository(SWP391_RedRibbonLifeContext dbContext, IRepository<TestType> testTypeRepository)
    {
        _dbContext = dbContext;
        _testTypeRepository = testTypeRepository;
    }

    public async Task<TestType> CreateTestTypeWithTransactionAsync(TestType testType)
    {
        await using var transaction = await _dbContext.Database.BeginTransactionAsync();
        try
        {
            testType.IsActive = true;
            var createdTestType = await _testTypeRepository.CreateAsync(testType);
            await transaction.CommitAsync();
            return createdTestType;
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<TestType> UpdateTestTypeWithTransactionAsync(TestType testType)
    {
        await using var transaction = await _dbContext.Database.BeginTransactionAsync();
        try
        {
            var updatedTestType = await _testTypeRepository.UpdateAsync(testType);
            await transaction.CommitAsync();
            return updatedTestType;
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<List<TestType>> GetAllTestTypesAsync()
    {
        return await _testTypeRepository.GetAllAsync();
    }

    public async Task<TestType?> GetTestTypeByIdAsync(int id, bool useNoTracking = true)
    {
        return await _testTypeRepository.GetAsync(t => t.TestTypeId == id, useNoTracking);
    }

    public async Task<TestType?> GetTestTypeForUpdateAsync(int id)
    {
        return await _testTypeRepository.GetAsync(t => t.TestTypeId == id, false);
    }

    public async Task<bool> DeleteTestTypeWithTransactionAsync(TestType testType)
    {
        await using var transaction = await _dbContext.Database.BeginTransactionAsync();
        try
        {
            await _testTypeRepository.DeleteAsync(testType);
            await transaction.CommitAsync();
            return true;
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
}