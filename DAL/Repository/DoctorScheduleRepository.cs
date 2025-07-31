using DAL.IRepository;
using DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace DAL.Repository;

public class DoctorScheduleRepository : IDoctorScheduleRepository
{
    private readonly SWP391_RedRibbonLifeContext _dbContext;
    private readonly IRepository<DoctorSchedule> _doctorScheduleRepository;
    
    public DoctorScheduleRepository(SWP391_RedRibbonLifeContext dbContext, IRepository<DoctorSchedule> doctorScheduleRepository)
    {
        _dbContext = dbContext;
        _doctorScheduleRepository = doctorScheduleRepository;
    }
    
    public async Task<DoctorSchedule?> GetDoctorScheduleToCheckAsync(int id, string day)
    {
        return await _doctorScheduleRepository.GetAsync(
            u => u.DoctorId == id && u.WorkDay == day, 
            true
        );
    }

    public async Task<DoctorSchedule> CreateDoctorScheduleWithTransactionAsync(DoctorSchedule doctorSchedule)
    {
        await using var transaction = await _dbContext.Database.BeginTransactionAsync();
        try
        {
            var createdDoctorSchedule = await _doctorScheduleRepository.CreateAsync(doctorSchedule);
            await transaction.CommitAsync();
            return createdDoctorSchedule;
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<DoctorSchedule> UpdateDoctorScheduleWithTransactionAsync(DoctorSchedule doctorSchedule)
    {
        await using var transaction = await _dbContext.Database.BeginTransactionAsync();
        try
        {
            var updatedDoctorSchedule = await _doctorScheduleRepository.UpdateAsync(doctorSchedule);
            await transaction.CommitAsync();
            return updatedDoctorSchedule;
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<DoctorSchedule?> GetDoctorScheduleWithRelationsAsync(int scheduleId, bool useNoTracking = true)
    {
        return await _doctorScheduleRepository.GetWithRelationsAsync(
            filter: ds => ds.ScheduleId == scheduleId,
            useNoTracking: useNoTracking,
            includeFunc: query => query
                .Include(ds => ds.Doctor)
                    .ThenInclude(d => d.User)
        );
    }

    public async Task<DoctorSchedule?> GetDoctorScheduleForUpdateAsync(int scheduleId)
    {
        return await _doctorScheduleRepository.GetAsync(d => d.ScheduleId == scheduleId, false);
    }

    public async Task<List<DoctorSchedule>> GetAllDoctorSchedulesByDoctorIdAsync(int doctorId)
    {
        return await _doctorScheduleRepository.GetAllWithRelationsByFilterAsync(
            filter: ds => ds.DoctorId == doctorId,
            useNoTracking: true,
            includeFunc: query => query
                .Include(ds => ds.Doctor)
                    .ThenInclude(d => d.User)
        );
    }

    public async Task<bool> DeleteDoctorScheduleWithTransactionAsync(DoctorSchedule doctorSchedule)
    {
        await using var transaction = await _dbContext.Database.BeginTransactionAsync();
        try
        {
            var result = await _doctorScheduleRepository.DeleteAsync(doctorSchedule);
            await transaction.CommitAsync();
            return result;
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
}