using AutoMapper;
using BLL.DTO.Appointment;
using BLL.Interfaces;
using DAL.IRepository;
using DAL.Models;

namespace BLL.Services;

public class AppointmentService : IAppointmentService
{
    private readonly IUserRepository<Appointment> _appointmentRepository;
    private readonly IUserRepository<Patient> _patientRepository;
    private readonly IUserRepository<Doctor> _doctorRepository;
    private readonly IMapper _mapper;
    private readonly IUserUtils _userUtils;
    private readonly SWP391_RedRibbonLifeContext _dbContext;
    public AppointmentService(IUserRepository<Appointment> appointmentRepository, IUserRepository<Patient> patientRepository, IUserRepository<Doctor> doctorRepository, IMapper mapper, IUserUtils userUtils, SWP391_RedRibbonLifeContext dbContext)
    {
        _appointmentRepository = appointmentRepository;
        _patientRepository = patientRepository;
        _doctorRepository = doctorRepository;
        _mapper = mapper;
        _userUtils = userUtils;
        _dbContext = dbContext;
    }
    
    public async Task<dynamic> CreateAppointmentAsync(AppointmentCreateDTO dto)
    {
        ArgumentNullException.ThrowIfNull(dto, $"{nameof(dto)} is null");
        if (dto.AppointmentDate < DateOnly.FromDateTime(DateTime.Now))
        {
            throw new Exception("Appointment date cannot be in the past.");
        }
        if (dto.AppointmentTime < TimeOnly.FromDateTime(DateTime.Now))
        {
            throw new Exception("Appointment time cannot be in the past.");
        }
        // Check if patient and doctor exist
        _userUtils.CheckDoctorExist(dto.DoctorId);
        _userUtils.CheckPatientExist(dto.PatientId);
        // Begin transaction
        using var transaction = await _dbContext.Database.BeginTransactionAsync();
        try
        {
            // Create appointment
            Appointment appointment = _mapper.Map<Appointment>(dto);
            appointment.Status = "Scheduled";
            var createdAppointment = await _appointmentRepository.CreateAsync(appointment);
            // Commit transaction
            await transaction.CommitAsync();
            return new
            {
                AppointmentInfo = _mapper.Map<AppointmentDTO>(createdAppointment)
            };
        }
        catch (Exception)
        {
            // Rollback transaction on error
            await transaction.RollbackAsync();
            throw; // Re-throw the exception
        }
    }

    
}