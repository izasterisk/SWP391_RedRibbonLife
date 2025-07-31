using DAL.Models;

namespace DAL.IRepository;

public interface IAppointmentRepository
{
    Task<Appointment?> GetAppointmentToCheckAsync(int id, DateOnly date, TimeOnly time);
    Task<Appointment?> GetAppointmentById4NotificationAsync(int appointmentId);
    Task<Appointment> CreateAppointmentWithTransactionAsync(Appointment appointment);
    Task<Appointment> UpdateAppointmentWithTransactionAsync(Appointment appointment);
    Task<bool> DeleteAppointmentWithTransactionAsync(Appointment appointment);
    Task<Appointment?> GetAppointmentWithRelationsAsync(int appointmentId, bool useNoTracking = true);
    Task<Appointment?> GetAppointmentForUpdateAsync(int appointmentId);
    Task<List<Appointment>> GetAppointmentsByPatientIdAsync(int patientId);
    Task<List<Appointment>> GetAppointmentsByDoctorIdAsync(int doctorId);
    Task<List<Appointment>> GetScheduledAppointmentsPaginatedAsync(int page, int pageSize);
    Task<int> CountScheduledAppointmentsAsync();
    Task<List<Doctor>> GetAllDoctorsWithUserAsync();
}