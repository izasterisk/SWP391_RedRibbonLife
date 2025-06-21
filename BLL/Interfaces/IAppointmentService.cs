using BLL.DTO.Appointment;

namespace BLL.Interfaces;

public interface IAppointmentService
{
    Task<dynamic> CreateAppointmentAsync(AppointmentCreateDTO dto);
}