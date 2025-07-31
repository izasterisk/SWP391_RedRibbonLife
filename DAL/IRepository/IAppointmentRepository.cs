using DAL.Models;

namespace DAL.IRepository;

public interface IAppointmentRepository
{
    Task<Appointment?> GetAppointmentToCheckAsync(int id, DateOnly date, TimeOnly time);
    Task<Appointment?> GetAppointmentById4NotificationAsync(int appointmentId);
}