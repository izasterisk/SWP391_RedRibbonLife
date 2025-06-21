using BLL.DTO.Appointment;

namespace BLL.Interfaces;

public interface IAppointmentService
{
    Task<dynamic> CreateAppointmentAsync(AppointmentCreateDTO dto);
    Task<dynamic> UpdateAppointmentAsync(AppointmentUpdateDTO dto);
    Task<List<AppointmentReadOnlyDTO>> GetAllAppointmentsByPatientIdAsync(int id);
    Task<List<AppointmentReadOnlyDTO>> GetAllAppointmentsByDoctorIdAsync(int id);
    Task<dynamic> GetAvailableDoctorsAsync(DateOnly appointmentDate, TimeOnly appointmentTime);
    Task<bool> DeleteAppointmentByIdAsync(int id);
}