using DAL.IRepository;
using DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace DAL.Repository;

public class AppointmentRepository : IAppointmentRepository
{
    private readonly SWP391_RedRibbonLifeContext _dbContext;
    private readonly IRepository<Appointment> _appointmentRepository;
    private readonly IRepository<Doctor> _doctorRepository;
    
    public AppointmentRepository(SWP391_RedRibbonLifeContext dbContext, IRepository<Appointment> appointmentRepository, IRepository<Doctor> doctorRepository)
    {
        _dbContext = dbContext;
        _appointmentRepository = appointmentRepository;
        _doctorRepository = doctorRepository;
    }
    
    public async Task<Appointment?> GetAppointmentToCheckAsync(int id, DateOnly date, TimeOnly time)
    {
        return await _appointmentRepository.GetAsync(
            u => u.DoctorId == id && u.AppointmentDate == date 
            && u.AppointmentTime == time 
            && (u.Status == "Confirmed" || u.Status == "Scheduled"), true);
    }

    public async Task<Appointment?> GetAppointmentById4NotificationAsync(int appointmentId)
    {
        return await _appointmentRepository.GetWithRelationsAsync(
            a => a.AppointmentId == appointmentId,
            useNoTracking: true,
            includeFunc: query => query
                .Include(a => a.Patient)
                .ThenInclude(p => p!.User)
                .Include(a => a.Doctor)
                .ThenInclude(d => d!.User)
        );
    }

    public async Task<Appointment> CreateAppointmentWithTransactionAsync(Appointment appointment)
    {
        await using var transaction = await _dbContext.Database.BeginTransactionAsync();
        try
        {
            appointment.Status = "Scheduled";
            var createdAppointment = await _appointmentRepository.CreateAsync(appointment);
            await transaction.CommitAsync();
            return createdAppointment;
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<Appointment> UpdateAppointmentWithTransactionAsync(Appointment appointment)
    {
        await using var transaction = await _dbContext.Database.BeginTransactionAsync();
        try
        {
            var updatedAppointment = await _appointmentRepository.UpdateAsync(appointment);
            await transaction.CommitAsync();
            return updatedAppointment;
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<bool> DeleteAppointmentWithTransactionAsync(Appointment appointment)
    {
        await using var transaction = await _dbContext.Database.BeginTransactionAsync();
        try
        {
            var result = await _appointmentRepository.DeleteAsync(appointment);
            await transaction.CommitAsync();
            return result;
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<Appointment?> GetAppointmentWithRelationsAsync(int appointmentId, bool useNoTracking = true)
    {
        return await _appointmentRepository.GetWithRelationsAsync(
            filter: a => a.AppointmentId == appointmentId,
            useNoTracking: useNoTracking,
            includeFunc: query => query
                .Include(a => a.Patient)
                    .ThenInclude(p => p!.User)
                .Include(a => a.Doctor)
                    .ThenInclude(d => d!.User)
                .Include(a => a.TestType)
        );
    }

    public async Task<Appointment?> GetAppointmentForUpdateAsync(int appointmentId)
    {
        return await _appointmentRepository.GetWithRelationsAsync(
            filter: a => a.AppointmentId == appointmentId,
            useNoTracking: false,
            includeFunc: query => query
                .Include(a => a.Patient)
                .ThenInclude(p => p!.User)
                .Include(a => a.Doctor)
                .ThenInclude(d => d!.User)
                .Include(a => a.TestType)
        );
    }

    public async Task<List<Appointment>> GetAppointmentsByPatientIdAsync(int patientId)
    {
        return await _appointmentRepository.GetAllWithRelationsAsync(
            includeFunc: query => query
                .Include(a => a.Patient)
                    .ThenInclude(p => p!.User)
                .Include(a => a.Doctor)
                    .ThenInclude(d => d!.User)
                .Include(a => a.TestType)
                .Where(a => a.PatientId == patientId)
        );
    }

    public async Task<List<Appointment>> GetAppointmentsByDoctorIdAsync(int doctorId)
    {
        return await _appointmentRepository.GetAllWithRelationsAsync(
            includeFunc: query => query
                .Include(a => a.Patient)
                    .ThenInclude(p => p!.User)
                .Include(a => a.Doctor)
                    .ThenInclude(d => d!.User)
                .Include(a => a.TestType)
                .Where(a => a.DoctorId == doctorId)
        );
    }

    public async Task<List<Appointment>> GetScheduledAppointmentsPaginatedAsync(int page, int pageSize)
    {
        return await _appointmentRepository.GetAllWithRelationsAsync(
            includeFunc: query => query
                .Include(a => a.Patient)
                    .ThenInclude(p => p!.User)
                .Include(a => a.Doctor)
                    .ThenInclude(d => d!.User)
                .Include(a => a.TestType)
                .Where(a => a.Status == "Scheduled")
                .OrderBy(a => Math.Abs((a.AppointmentDate.ToDateTime(a.AppointmentTime) - DateTime.Now).TotalMinutes))
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
        );
    }

    public async Task<int> CountScheduledAppointmentsAsync()
    {
        return await _appointmentRepository.CountAsync(a => a.Status == "Scheduled");
    }

    public async Task<List<Doctor>> GetAllDoctorsWithUserAsync()
    {
        return await _doctorRepository.GetAllWithRelationsAsync(
            includeFunc: query => query.Include(d => d.User)
        );
    }
}