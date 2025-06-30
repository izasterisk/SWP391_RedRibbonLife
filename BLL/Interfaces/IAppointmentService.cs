using BLL.DTO.Appointment;

namespace BLL.Interfaces;

public interface IAppointmentService
{
    Task<AppointmentReadOnlyDTO> CreateAppointmentAsync(AppointmentCreateDTO dto);
    Task<AppointmentReadOnlyDTO> UpdateAppointmentAsync(AppointmentUpdateDTO dto);
    Task<List<AppointmentReadOnlyDTO>> GetAllAppointmentsByPatientIdAsync(int id);
    Task<List<AppointmentReadOnlyDTO>> GetAllAppointmentsByDoctorIdAsync(int id);
    Task<AppointmentReadOnlyDTO> GetAppointmentByIdAsync(int id);
    Task<dynamic> GetAvailableDoctorsAsync(DateOnly appointmentDate, TimeOnly appointmentTime);
    // Task<bool> DeleteAppointmentByIdAsync(int id);
}