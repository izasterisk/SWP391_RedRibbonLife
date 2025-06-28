using AutoMapper;
using BLL.DTO.Appointment;
using BLL.Interfaces;
using DAL.IRepository;
using DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace BLL.Services;

public class AppointmentService : IAppointmentService
{
    private readonly IUserRepository<Appointment> _appointmentRepository;
    private readonly IUserRepository<Patient> _patientRepository;
    private readonly IUserRepository<Doctor> _doctorRepository;
    private readonly IUserRepository<DoctorSchedule> _doctorScheduleRepository;
    private readonly IMapper _mapper;
    private readonly IUserUtils _userUtils;
    private readonly IDoctorScheduleUtils _doctorScheduleUtils;
    private readonly SWP391_RedRibbonLifeContext _dbContext;
    public AppointmentService(IUserRepository<Appointment> appointmentRepository, IUserRepository<Patient> patientRepository, IUserRepository<Doctor> doctorRepository, IUserRepository<DoctorSchedule> doctorScheduleRepository, IMapper mapper, IUserUtils userUtils, IDoctorScheduleUtils doctorScheduleUtils, SWP391_RedRibbonLifeContext dbContext)
    {
        _appointmentRepository = appointmentRepository;
        _patientRepository = patientRepository;
        _doctorRepository = doctorRepository;
        _doctorScheduleRepository = doctorScheduleRepository;
        _mapper = mapper;
        _userUtils = userUtils;
        _doctorScheduleUtils = doctorScheduleUtils;
        _dbContext = dbContext;
    }
    
    public async Task<dynamic> CreateAppointmentAsync(AppointmentCreateDTO dto)
    {
        ArgumentNullException.ThrowIfNull(dto, $"{nameof(dto)} is null");
        _userUtils.CheckDoctorExist(dto.DoctorId);
        _userUtils.CheckPatientExist(dto.PatientId);
        _doctorScheduleUtils.CheckDoctorIfAvailable(dto.DoctorId, dto.AppointmentDate, dto.AppointmentTime);
        // Begin transaction
        using var transaction = await _dbContext.Database.BeginTransactionAsync();
        try
        {
            // Create appointment
            Appointment appointment = _mapper.Map<Appointment>(dto);
            appointment.Status = "Scheduled";
            var createdAppointment = await _appointmentRepository.CreateAsync(appointment);
            // Load appointment với navigation properties để lấy PatientName và DoctorName
            var appointmentWithNavigations = await _dbContext.Appointments
                .Include(a => a.Patient).ThenInclude(p => p.User)
                .Include(a => a.Doctor).ThenInclude(d => d.User)
                .FirstOrDefaultAsync(a => a.AppointmentId == createdAppointment.AppointmentId);
            // Commit transaction
            await transaction.CommitAsync();
            // Sử dụng AutoMapper để map appointment với navigation properties
            return _mapper.Map<AppointmentReadOnlyDTO>(appointmentWithNavigations);
        }
        catch (Exception)
        {
            // Rollback transaction on error
            await transaction.RollbackAsync();
            throw; // Re-throw the exception
        }
    }

    public async Task<dynamic> UpdateAppointmentAsync(AppointmentUpdateDTO dto)
    {
        ArgumentNullException.ThrowIfNull(dto, $"{nameof(dto)} is null");
        // Validate AppointmentType nếu có giá trị
        if (!string.IsNullOrEmpty(dto.AppointmentType))
        {
            var allowedTypes = new[] { "Appointment", "Medication" };
            if (!allowedTypes.Contains(dto.AppointmentType))
            {
                throw new ArgumentException("Appointment type must be either 'Appointment' or 'Medication'");
            }
        }
    
        // Validate Status nếu có giá trị
        if (!string.IsNullOrEmpty(dto.Status))
        {
            var allowedStatuses = new[] { "Scheduled", "Confirmed", "Completed", "Cancelled" };
            if (!allowedStatuses.Contains(dto.Status))
            {
                throw new ArgumentException("Status must be one of: Scheduled, Confirmed, Completed, Cancelled");
            }
        }
        // Begin transaction for consistency
        using var transaction = await _dbContext.Database.BeginTransactionAsync();
        try
        {
            // Get existing appointment with navigation properties
            var appointment = await _dbContext.Appointments
                .Include(a => a.Patient).ThenInclude(p => p.User)
                .Include(a => a.Doctor).ThenInclude(d => d.User)
                .FirstOrDefaultAsync(a => a.AppointmentId == dto.AppointmentId);
            if (appointment == null)
            {
                throw new Exception("Appointment not found.");
            }
            // Validate doctor availability if schedule-related fields are changing
            var finalDate = dto.AppointmentDate ?? appointment.AppointmentDate;
            var finalTime = dto.AppointmentTime ?? appointment.AppointmentTime;
            if (dto.DoctorId != null)
            {
                _userUtils.CheckDoctorExist(dto.DoctorId.Value);
                _doctorScheduleUtils.CheckDoctorIfAvailable(dto.DoctorId.Value, finalDate, finalTime);
            }
            else if (dto.AppointmentDate != null || dto.AppointmentTime != null)
            {
                _doctorScheduleUtils.CheckDoctorIfAvailable(appointment.DoctorId, finalDate, finalTime);
            }
            // Update appointment using AutoMapper
            _mapper.Map(dto, appointment);
            // Save changes
            await _appointmentRepository.UpdateAsync(appointment);
            // Commit transaction
            await transaction.CommitAsync();
            // Return mapped DTO (appointment already has navigation properties loaded)
            return _mapper.Map<AppointmentReadOnlyDTO>(appointment);
        }
        catch (Exception)
        {
            // Rollback transaction on error
            await transaction.RollbackAsync();
            throw; // Re-throw the exception
        }
    }
    
    public async Task<List<AppointmentReadOnlyDTO>> GetAllAppointmentsByPatientIdAsync(int id)
    {
        var appointments = await _dbContext.Appointments
            .Include(a => a.Patient).ThenInclude(p => p.User)
            .Include(a => a.Doctor).ThenInclude(d => d.User)
            .Where(a => a.PatientId == id)
            .ToListAsync();
        return _mapper.Map<List<AppointmentReadOnlyDTO>>(appointments);
    }
    
    public async Task<List<AppointmentReadOnlyDTO>> GetAllAppointmentsByDoctorIdAsync(int id)
    {
        var appointments = await _dbContext.Appointments
            .Include(a => a.Patient).ThenInclude(p => p.User)
            .Include(a => a.Doctor).ThenInclude(d => d.User)
            .Where(a => a.DoctorId == id)
            .ToListAsync();
        return _mapper.Map<List<AppointmentReadOnlyDTO>>(appointments);
    }
    
    public async Task<dynamic> GetAvailableDoctorsAsync(DateOnly appointmentDate, TimeOnly appointmentTime)
    {
        var allDoctors = await _doctorRepository.GetAllWithRelationsAsync(
            query => query.Include(d => d.User)
        );
        var availableDoctors = new List<object>();
        foreach (var doctor in allDoctors)
        {
            try
            {
                _doctorScheduleUtils.CheckDoctorIfAvailable(doctor.DoctorId, appointmentDate, appointmentTime);
                availableDoctors.Add(new
                {
                    DoctorId = doctor.DoctorId,
                    DoctorName = doctor.User?.FullName
                });
            }
            catch
            {
                continue;
            }
        }
        return new
        {
            AppointmentDate = appointmentDate,
            AppointmentTime = appointmentTime,
            AvailableDoctors = availableDoctors
        };
    }
    
    public async Task<bool> DeleteAppointmentByIdAsync(int id)
    {
        var appointment = await _appointmentRepository.GetAsync(u => u.AppointmentId == id, true);
        if (appointment == null)
        {
            throw new Exception($"Appointment with ID {id} not found");
        }
        await _appointmentRepository.DeleteAsync(appointment);
        return true;
    }
}