namespace BLL.Interfaces;

public interface INotificationService
{
    Task SendAppointmentRemindersAsync();
    Task SendMedicationRemindersAsync(int frequency);
    Task ProcessPendingNotificationsAsync();
}