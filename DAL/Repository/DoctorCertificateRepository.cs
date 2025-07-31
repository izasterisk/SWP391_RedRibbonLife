using DAL.IRepository;
using DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace DAL.Repository;

public class DoctorCertificateRepository : IDoctorCertificateRepository
{
    private readonly SWP391_RedRibbonLifeContext _dbContext;
    private readonly IRepository<DoctorCertificate> _certificateRepository;

    public DoctorCertificateRepository(SWP391_RedRibbonLifeContext dbContext, IRepository<DoctorCertificate> certificateRepository)
    {
        _dbContext = dbContext;
        _certificateRepository = certificateRepository;
    }

    public async Task<DoctorCertificate> CreateDoctorCertificateWithTransactionAsync(DoctorCertificate certificate)
    {
        await using var transaction = await _dbContext.Database.BeginTransactionAsync();
        try
        {
            var created = await _certificateRepository.CreateAsync(certificate);
            await transaction.CommitAsync();
            return created;
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<DoctorCertificate> UpdateDoctorCertificateWithTransactionAsync(DoctorCertificate certificate)
    {
        await using var transaction = await _dbContext.Database.BeginTransactionAsync();
        try
        {
            var updated = await _certificateRepository.UpdateAsync(certificate);
            await transaction.CommitAsync();
            return updated;
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<DoctorCertificate?> GetDoctorCertificateWithRelationsAsync(int certificateId, bool useNoTracking = true)
    {
        return await _certificateRepository.GetWithRelationsAsync(
            filter: c => c.CertificateId == certificateId,
            useNoTracking: useNoTracking,
            includeFunc: query => query
                .Include(c => c.Doctor)
                    .ThenInclude(d => d!.User)
        );
    }

    public async Task<List<DoctorCertificate>> GetAllDoctorCertificatesWithRelationsAsync()
    {
        return await _certificateRepository.GetAllWithRelationsAsync(
            includeFunc: query => query
                .Include(c => c.Doctor)
                    .ThenInclude(d => d!.User)
        );
    }

    public async Task<List<DoctorCertificate>> GetDoctorCertificatesByDoctorIdWithRelationsAsync(int doctorId, bool useNoTracking = true)
    {
        return await _certificateRepository.GetAllWithRelationsByFilterAsync(
            filter: c => c.DoctorId == doctorId,
            useNoTracking: useNoTracking,
            includeFunc: query => query
                .Include(c => c.Doctor)
                    .ThenInclude(d => d!.User)
        );
    }

    public async Task<DoctorCertificate?> GetDoctorCertificateForUpdateAsync(int certificateId)
    {
        return await _certificateRepository.GetAsync(c => c.CertificateId == certificateId, false);
    }

    public async Task<bool> DeleteDoctorCertificateWithTransactionAsync(DoctorCertificate certificate)
    {
        await using var transaction = await _dbContext.Database.BeginTransactionAsync();
        try
        {
            var result = await _certificateRepository.DeleteAsync(certificate);
            await transaction.CommitAsync();
            return result;
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
}