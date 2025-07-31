using DAL.IRepository;
using DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace DAL.Repository;

public class TestResultRepository : ITestResultRepository
{
    private readonly SWP391_RedRibbonLifeContext _dbContext;
    private readonly IRepository<TestResult> _testResultRepository;
    private readonly IRepository<Appointment> _appointmentRepository;

    public TestResultRepository(SWP391_RedRibbonLifeContext dbContext, IRepository<TestResult> testResultRepository, IRepository<Appointment> appointmentRepository)
    {
        _dbContext = dbContext;
        _testResultRepository = testResultRepository;
        _appointmentRepository = appointmentRepository;
    }

    public async Task<TestResult> CreateTestResultWithTransactionAsync(TestResult testResult, Appointment appointment)
    {
        await using var transaction = await _dbContext.Database.BeginTransactionAsync();
        try
        {
            var createdTestResult = await _testResultRepository.CreateAsync(testResult);
            appointment.Status = "Completed";
            await _appointmentRepository.UpdateAsync(appointment);
            await transaction.CommitAsync();
            
            var fullTestResult = await _testResultRepository.GetWithRelationsAsync(
                t => t.TestResultId == createdTestResult.TestResultId, 
                useNoTracking: true,
                includeFunc: query => query.Include(t => t.TestType)
                              .Include(t => t.Patient).ThenInclude(p => p.User)
                              .Include(t => t.Doctor).ThenInclude(d => d.User)
                              .Include(t => t.Appointment)
            );
            return fullTestResult ?? createdTestResult;
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<TestResult> UpdateTestResultAsync(TestResult testResult)
    {
        await using var transaction = await _dbContext.Database.BeginTransactionAsync();
        try
        {
            var updatedTestResult = await _testResultRepository.UpdateAsync(testResult);
            await transaction.CommitAsync();
            
            var fullTestResult = await _testResultRepository.GetWithRelationsAsync(
                t => t.TestResultId == updatedTestResult.TestResultId, 
                useNoTracking: true,
                includeFunc: query => query.Include(t => t.TestType)
                              .Include(t => t.Patient).ThenInclude(p => p.User)
                              .Include(t => t.Doctor).ThenInclude(d => d.User)
                              .Include(t => t.Appointment)
            );
            return fullTestResult ?? updatedTestResult;
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<List<TestResult>> GetAllTestResultsWithRelationsAsync()
    {
        return await _testResultRepository.GetAllWithRelationsAsync(
            includeFunc: query => query.Include(t => t.TestType)
                          .Include(t => t.Patient).ThenInclude(p => p.User)
                          .Include(t => t.Doctor).ThenInclude(d => d.User)
                          .Include(t => t.Appointment)
        );
    }

    public async Task<TestResult?> GetTestResultWithRelationsAsync(int id)
    {
        return await _testResultRepository.GetWithRelationsAsync(
            filter: t => t.TestResultId == id, 
            useNoTracking: true,
            includeFunc: query => query.Include(t => t.TestType)
                          .Include(t => t.Patient).ThenInclude(p => p.User)
                          .Include(t => t.Doctor).ThenInclude(d => d.User)
                          .Include(t => t.Appointment)
        );
    }

    public async Task<List<TestResult>> GetTestResultsByDoctorIdWithRelationsAsync(int doctorId)
    {
        return await _testResultRepository.GetAllWithRelationsByFilterAsync(
            filter: t => t.DoctorId == doctorId,
            useNoTracking: true,
            includeFunc: query => query.Include(t => t.TestType)
                          .Include(t => t.Patient).ThenInclude(p => p.User)
                          .Include(t => t.Doctor).ThenInclude(d => d.User)
                          .Include(t => t.Appointment)
        );
    }

    public async Task<TestResult?> GetTestResultForUpdateAsync(int id)
    {
        return await _testResultRepository.GetAsync(t => t.TestResultId == id, false);
    }

    public async Task<bool> DeleteTestResultAsync(TestResult testResult)
    {
        await _testResultRepository.DeleteAsync(testResult);
        return true;
    }
}