using AutoMapper;
using BLL.DTO;
using BLL.DTO.Appointment;
using BLL.Interfaces;
using BLL.Utils;
using DAL.IRepository;
using DAL.Models;

namespace BLL.Services;

public class AppointmentService : IAppointmentService
{
    private readonly IMapper _mapper;
    private readonly IAppointmentRepository _appointmentRepository;
    private readonly IUserUtils _userUtils;
    private readonly IDoctorScheduleUtils _doctorScheduleUtils;
    private readonly SendGridEmailUtil _sendGridUtil;
    
    public AppointmentService(IMapper mapper, IAppointmentRepository appointmentRepository, IUserUtils userUtils, IDoctorScheduleUtils doctorScheduleUtils, SendGridEmailUtil sendGridUtil)
    {
        _mapper = mapper;
        _appointmentRepository = appointmentRepository;
        _userUtils = userUtils;
        _doctorScheduleUtils = doctorScheduleUtils;
        _sendGridUtil = sendGridUtil;
    }
    
    public async Task<AppointmentReadOnlyDTO> CreateAppointmentAsync(AppointmentCreateDTO dto)
    {
        ArgumentNullException.ThrowIfNull(dto, $"{nameof(dto)} is null");
        await _userUtils.CheckDoctorExistAsync(dto.DoctorId);
        await _userUtils.CheckPatientExistAsync(dto.PatientId);
        await _doctorScheduleUtils.CheckDoctorIfAvailableAsync(dto.DoctorId, dto.AppointmentDate, dto.AppointmentTime);
        if (dto.AppointmentType == "Medication" && dto.TestTypeId == null)
        {
            throw new Exception("Test type is required for medication appointment.");
        }
        if (dto.AppointmentType == "Appointment" && dto.TestTypeId != null)
        {
            throw new Exception("Appointment type is Appointment cannot have test type.");
        }
        await dto.TestTypeId.ValidateIfNotNullAsync(_userUtils.CheckTestTypeExistAsync);

        Appointment appointment = _mapper.Map<Appointment>(dto);
        var createdAppointment = await _appointmentRepository.CreateAppointmentWithTransactionAsync(appointment);
        var detailedAppointment = await _appointmentRepository.GetAppointmentWithRelationsAsync(createdAppointment.AppointmentId, true);
        return _mapper.Map<AppointmentReadOnlyDTO>(detailedAppointment);
    }

    public async Task<AppointmentReadOnlyDTO> UpdateAppointmentAsync(AppointmentUpdateDTO dto)
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
        await dto.TestTypeId.ValidateIfNotNullAsync(_userUtils.CheckTestTypeExistAsync);
        var appointment = await _appointmentRepository.GetAppointmentForUpdateAsync(dto.AppointmentId);
        if (appointment == null)
        {
            throw new Exception("Appointment not found.");
        }
        if (dto.AppointmentType == "Medication" && dto.TestTypeId == null && appointment.TestTypeId == null)
        {
            throw new Exception("Test type is required for medication appointment.");
        }
        if (dto.AppointmentType == "Appointment")
        {
            if (appointment.TestTypeId != null)
                dto.TestTypeId = null;
            if (dto.TestTypeId != null)
                throw new Exception("Appointment type is Appointment cannot have test type.");
        }
        var finalDate = dto.AppointmentDate ?? appointment.AppointmentDate;
        var finalTime = dto.AppointmentTime ?? appointment.AppointmentTime;
        if (dto.DoctorId != null)
        {
            await _userUtils.CheckDoctorExistAsync(dto.DoctorId.Value);
            await _doctorScheduleUtils.CheckDoctorIfAvailableAsync(dto.DoctorId.Value, finalDate, finalTime);
        }
        else if (dto.AppointmentDate != null || dto.AppointmentTime != null)
        {
            await _doctorScheduleUtils.CheckDoctorIfAvailableAsync(appointment.DoctorId, finalDate, finalTime);
        }

        _mapper.Map(dto, appointment);
        var updatedAppointment = await _appointmentRepository.UpdateAppointmentWithTransactionAsync(appointment);
        if(dto.Status == "Confirmed")
        {
            await _sendGridUtil.SendAppointmentApprovalEmailAsync(appointment.Patient!.User!.Email!, appointment.Patient.User.FullName!, finalDate, finalTime);
        }
        var detailedAppointment = await _appointmentRepository.GetAppointmentWithRelationsAsync(updatedAppointment.AppointmentId, true);
        return _mapper.Map<AppointmentReadOnlyDTO>(detailedAppointment);
    }
    
    public async Task<List<AppointmentReadOnlyDTO>> GetAllAppointmentsByPatientIdAsync(int id)
    {
        var appointments = await _appointmentRepository.GetAppointmentsByPatientIdAsync(id);
        return _mapper.Map<List<AppointmentReadOnlyDTO>>(appointments);
    }
    
    public async Task<List<AppointmentReadOnlyDTO>> GetAllAppointmentsByDoctorIdAsync(int id)
    {
        var appointments = await _appointmentRepository.GetAppointmentsByDoctorIdAsync(id);
        return _mapper.Map<List<AppointmentReadOnlyDTO>>(appointments);
    }
    
    public async Task<dynamic> GetAvailableDoctorsAsync(DateOnly appointmentDate, TimeOnly appointmentTime)
    {
        var allDoctors = await _appointmentRepository.GetAllDoctorsWithUserAsync();
        var availableDoctors = new List<object>();
        foreach (var doctor in allDoctors)
        {
            try
            {
                await _doctorScheduleUtils.CheckDoctorIfAvailableAsync(doctor.DoctorId, appointmentDate, appointmentTime);
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
    
    public async Task<AppointmentReadOnlyDTO> GetAppointmentByIdAsync(int id)
    {
        var appointment = await _appointmentRepository.GetAppointmentWithRelationsAsync(id, true);
        if (appointment == null)
        {
            throw new Exception($"Appointment with ID {id} not found");
        }
        return _mapper.Map<AppointmentReadOnlyDTO>(appointment);
    }
    
    public async Task<PagedResponse<AppointmentReadOnlyDTO>> GetAllScheduledAppointmentsAsync(int page = 1, int pageSize = 10)
    {
        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 10;
        if (pageSize > 100) pageSize = 100;
        var totalRecords = await _appointmentRepository.CountScheduledAppointmentsAsync();
        var totalPages = (int)Math.Ceiling((double)totalRecords / pageSize);
        var appointments = await _appointmentRepository.GetScheduledAppointmentsPaginatedAsync(page, pageSize);
        var appointmentDTOs = _mapper.Map<List<AppointmentReadOnlyDTO>>(appointments);
        return new PagedResponse<AppointmentReadOnlyDTO>
        {
            Data = appointmentDTOs,
            CurrentPage = page,
            PageSize = pageSize,
            TotalPages = totalPages,
            TotalRecords = totalRecords,
            HasNextPage = page < totalPages,
            HasPreviousPage = page > 1
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