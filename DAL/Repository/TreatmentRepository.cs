using DAL.IRepository;
using DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace DAL.Repository;

public class TreatmentRepository : ITreatmentRepository
{
    private readonly SWP391_RedRibbonLifeContext _dbContext;
    private readonly IRepository<Treatment> _treatmentRepository;
    
    public TreatmentRepository(SWP391_RedRibbonLifeContext dbContext, IRepository<Treatment> treatmentRepository)
    {
        _dbContext = dbContext;
        _treatmentRepository = treatmentRepository;
    }
    
    public async Task<Treatment?> GetTreatmentToCheckAsync(int id)
    {
        return await _treatmentRepository.GetAsync(u => u.RegimenId == id, true);
    }
    
    public async Task<Treatment?> GetTreatmentById4NotificationAsync(int id)
    {
        return await _treatmentRepository.GetWithRelationsAsync(
            t => t.TreatmentId == id,
            useNoTracking: true,
            includeFunc: query => query
                .Include(t => t.Regimen)
                .Include(t => t.TestResult)
                .ThenInclude(tr => tr.Patient)
                .ThenInclude(p => p.User)
        );
    }
    
    public async Task<bool> CheckTreatmentExistsByTestResultIdAsync(int testResultId)
    {
        return await _treatmentRepository.AnyAsync(t => t.TestResultId == testResultId);
    }
    
    public async Task<Treatment> CreateTreatmentWithTransactionAsync(Treatment treatment)
    {
        await using var transaction = await _dbContext.Database.BeginTransactionAsync();
        try
        {
            var createdTreatment = await _treatmentRepository.CreateAsync(treatment);
            await transaction.CommitAsync();
            return createdTreatment;
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
    
    public async Task<Treatment> UpdateTreatmentWithTransactionAsync(Treatment treatment)
    {
        await using var transaction = await _dbContext.Database.BeginTransactionAsync();
        try
        {
            var updatedTreatment = await _treatmentRepository.UpdateAsync(treatment);
            await transaction.CommitAsync();
            return updatedTreatment;
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
    
    public async Task<List<Treatment>> GetAllTreatmentsWithDetailsAsync()
    {
        return await _treatmentRepository.GetAllWithRelationsAsync(
            includeFunc: query => query
                .Include(t => t.Regimen)
                .ThenInclude(tr => tr.Component1)
                .Include(t => t.Regimen)
                .ThenInclude(tr => tr.Component2)
                .Include(t => t.Regimen)
                .ThenInclude(tr => tr.Component3)
                .Include(t => t.Regimen)
                .ThenInclude(tr => tr.Component4)
                .Include(t => t.TestResult)
                    .ThenInclude(tr => tr.Patient)
                    .ThenInclude(p => p.User)
                .Include(t => t.TestResult)
                    .ThenInclude(tr => tr.Doctor)
                    .ThenInclude(d => d.User)
                .Include(t => t.TestResult)
                    .ThenInclude(tr => tr.Appointment)
                .Include(t => t.TestResult)
                    .ThenInclude(tr => tr.TestType)
        );
    }
    
    public async Task<Treatment?> GetTreatmentWithDetailsByIdAsync(int treatmentId)
    {
        return await _treatmentRepository.GetWithRelationsAsync(
            filter: t => t.TreatmentId == treatmentId,
            useNoTracking: true,
            includeFunc: query => query
                .Include(t => t.Regimen)
                .ThenInclude(tr => tr.Component1)
                .Include(t => t.Regimen)
                .ThenInclude(tr => tr.Component2)
                .Include(t => t.Regimen)
                .ThenInclude(tr => tr.Component3)
                .Include(t => t.Regimen)
                .ThenInclude(tr => tr.Component4)
                .Include(t => t.TestResult)
                    .ThenInclude(tr => tr.Patient)
                    .ThenInclude(p => p.User)
                .Include(t => t.TestResult)
                    .ThenInclude(tr => tr.Doctor)
                    .ThenInclude(d => d.User)
                .Include(t => t.TestResult)
                    .ThenInclude(tr => tr.Appointment)
                .Include(t => t.TestResult)
                    .ThenInclude(tr => tr.TestType)
        );
    }
    
    public async Task<Treatment?> GetTreatmentWithDetailsByTestResultIdAsync(int testResultId)
    {
        return await _treatmentRepository.GetWithRelationsAsync(
            filter: t => t.TestResultId == testResultId,
            useNoTracking: true,
            includeFunc: query => query
                .Include(t => t.TestResult)
                    .ThenInclude(tr => tr.Patient)
                    .ThenInclude(p => p.User)
                .Include(t => t.TestResult)
                    .ThenInclude(tr => tr.Doctor)
                    .ThenInclude(d => d.User)
                .Include(t => t.TestResult)
                    .ThenInclude(tr => tr.Appointment)
                .Include(t => t.TestResult)
                    .ThenInclude(tr => tr.TestType)
        );
    }
    
    public async Task<List<Treatment>> GetTreatmentsByPatientIdAsync(int patientId)
    {
        return await _treatmentRepository.GetAllWithRelationsByFilterAsync(
            filter: t => t.TestResult.PatientId == patientId,
            useNoTracking: true,
            includeFunc: query => query
                .Include(t => t.Regimen)
                .ThenInclude(tr => tr.Component1)
                .Include(t => t.Regimen)
                .ThenInclude(tr => tr.Component2)
                .Include(t => t.Regimen)
                .ThenInclude(tr => tr.Component3)
                .Include(t => t.Regimen)
                .ThenInclude(tr => tr.Component4)
                .Include(t => t.TestResult)
                    .ThenInclude(tr => tr.Patient)
                    .ThenInclude(p => p.User)
                .Include(t => t.TestResult)
                    .ThenInclude(tr => tr.Doctor)
                    .ThenInclude(d => d.User)
                .Include(t => t.TestResult)
                    .ThenInclude(a => a.Appointment)
                .Include(t => t.TestResult)
                    .ThenInclude(a => a.TestType)
        );
    }
    
    public async Task<Treatment?> GetTreatmentForUpdateAsync(int treatmentId)
    {
        return await _treatmentRepository.GetAsync(t => t.TreatmentId == treatmentId, false);
    }
    
    public async Task<bool> DeleteTreatmentWithTransactionAsync(Treatment treatment)
    {
        await using var transaction = await _dbContext.Database.BeginTransactionAsync();
        try
        {
            await _treatmentRepository.DeleteAsync(treatment);
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