using AutoMapper;
using BLL.DTO.Appointment;
using BLL.Interfaces;
using DAL.IRepository;
using DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace BLL.Services;

public class AppointmentService : IAppointmentService
{
    private readonly IMapper _mapper;
    private readonly SWP391_RedRibbonLifeContext _dbContext;
    private readonly IUserRepository<Appointment> _appointmentRepository;
    private readonly IUserRepository<Patient> _patientRepository;
    private readonly IUserRepository<Doctor> _doctorRepository;
    private readonly IUserRepository<DoctorSchedule> _doctorScheduleRepository;
    private readonly IUserUtils _userUtils;
    private readonly IDoctorScheduleUtils _doctorScheduleUtils;
    
    public AppointmentService(IMapper mapper, SWP391_RedRibbonLifeContext dbContext, IUserRepository<Appointment> appointmentRepository, IUserRepository<Patient> patientRepository, IUserRepository<Doctor> doctorRepository, IUserRepository<DoctorSchedule> doctorScheduleRepository, IUserUtils userUtils, IDoctorScheduleUtils doctorScheduleUtils)
    {
        _mapper = mapper;
        _dbContext = dbContext;
        _appointmentRepository = appointmentRepository;
        _patientRepository = patientRepository;
        _doctorRepository = doctorRepository;
        _doctorScheduleRepository = doctorScheduleRepository;
        _userUtils = userUtils;
        _doctorScheduleUtils = doctorScheduleUtils;
    }
    
    public async Task<dynamic> CreateAppointmentAsync(AppointmentCreateDTO dto)
    {
        ArgumentNullException.ThrowIfNull(dto, $"{nameof(dto)} is null");
        _userUtils.CheckDoctorExist(dto.DoctorId);
        _userUtils.CheckPatientExist(dto.PatientId);
        _doctorScheduleUtils.CheckDoctorIfAvailable(dto.DoctorId, dto.AppointmentDate, dto.AppointmentTime);
        using var transaction = await _dbContext.Database.BeginTransactionAsync();
        try
        {
            Appointment appointment = _mapper.Map<Appointment>(dto);
            appointment.Status = "Scheduled";
            var createdAppointment = await _appointmentRepository.CreateAsync(appointment);
            await transaction.CommitAsync();
            var detailedAppointment = await _appointmentRepository.GetWithRelationsAsync(
                filter: a => a.AppointmentId == createdAppointment.AppointmentId,
                useNoTracking: true,
                includeFunc: query => query
                    .Include(a => a.Patient)
                        .ThenInclude(p => p.User)
                    .Include(a => a.Doctor)
                        .ThenInclude(d => d.User)
            );
            return _mapper.Map<AppointmentReadOnlyDTO>(detailedAppointment);
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<dynamic> UpdateAppointmentAsync(AppointmentUpdateDTO dto)
    {
        ArgumentNullException.ThrowIfNull(dto, $"{nameof(dto)} is null");
        if (!string.IsNullOrEmpty(dto.AppointmentType) && dto.AppointmentType != "Appointment" && dto.AppointmentType != "Medication")
        {
            throw new ArgumentException("Appointment type must be either 'Appointment' or 'Medication'");
        }
        if (!string.IsNullOrEmpty(dto.Status) && dto.Status != "Scheduled" && dto.Status != "Confirmed" && 
            dto.Status != "Completed" && dto.Status != "Cancelled")
        {
            throw new ArgumentException("Status must be one of: Scheduled, Confirmed, Completed, Cancelled");
        }
        using var transaction = await _dbContext.Database.BeginTransactionAsync();
        try
        {
            var appointment = await _appointmentRepository.GetWithRelationsAsync(
                filter: a => a.AppointmentId == dto.AppointmentId,
                useNoTracking: false,
                includeFunc: query => query
                    .Include(a => a.Patient)
                        .ThenInclude(p => p.User)
                    .Include(a => a.Doctor)
                        .ThenInclude(d => d.User)
            );
            if (appointment == null)
            {
                throw new Exception("Appointment not found.");
            }
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
            _mapper.Map(dto, appointment);
            var updatedAppointment = await _appointmentRepository.UpdateAsync(appointment);
            await transaction.CommitAsync();
            return _mapper.Map<AppointmentReadOnlyDTO>(updatedAppointment);
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
    
    public async Task<List<AppointmentReadOnlyDTO>> GetAllAppointmentsByPatientIdAsync(int id)
    {
        var appointments = await _appointmentRepository.GetAllWithRelationsAsync(
            includeFunc: query => query
                .Include(a => a.Patient)
                    .ThenInclude(p => p.User)
                .Include(a => a.Doctor)
                    .ThenInclude(d => d.User)
                .Where(a => a.PatientId == id)
        );
        return _mapper.Map<List<AppointmentReadOnlyDTO>>(appointments);
    }
    
    public async Task<List<AppointmentReadOnlyDTO>> GetAllAppointmentsByDoctorIdAsync(int id)
    {
        var appointments = await _appointmentRepository.GetAllWithRelationsAsync(
            includeFunc: query => query
                .Include(a => a.Patient)
                    .ThenInclude(p => p.User)
                .Include(a => a.Doctor)
                    .ThenInclude(d => d.User)
                .Where(a => a.DoctorId == id)
        );
        return _mapper.Map<List<AppointmentReadOnlyDTO>>(appointments);
    }
    
    public async Task<dynamic> GetAvailableDoctorsAsync(DateOnly appointmentDate, TimeOnly appointmentTime)
    {
        var allDoctors = await _doctorRepository.GetAllWithRelationsAsync(
            includeFunc: query => query.Include(d => d.User)
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
    
    // public async Task<bool> DeleteAppointmentByIdAsync(int id)
    // {
    //     var appointment = await _appointmentRepository.GetAsync(a => a.AppointmentId == id, true);
    //     if (appointment == null)
    //     {
    //         throw new Exception($"Appointment with ID {id} not found");
    //     }
    //     await _appointmentRepository.DeleteAsync(appointment);
    //     return true;
    // }
}