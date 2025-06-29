using BLL.DTO.Notification;
using DAL.Models;

namespace BLL.Interfaces;

public interface INotificationUtils
{
    Task<List<Appointment>> GetUpcomingAppointmentsAsync();
    Task<List<Treatment>> GetActiveTreatmentsForMedicationReminder(int frequency);
    Task<NotificationDTO> CreateNotificationAsync(NotificationCreateDTO dto);
    Task MarkNotificationAsSentAsync(int notificationId);
    Task MarkNotificationAsFailedAsync(int notificationId, string errorMessage);
    Task<List<Notification>> GetPendingNotificationsAsync();
    Task<NotificationDTO> UpdateErrorMessageAsync(NotificationUpdateDTO dto);
    Task<Appointment?> GetAppointmentByIdAsync(int appointmentId);
    Task<Treatment?> GetTreatmentByIdAsync(int treatmentId);
}