using BLL.Interfaces;

namespace BLL.Services;

public class HangfireBackgroundJobService : IHangfireBackgroundJobService
{
    private readonly INotificationService _notificationService;

    public HangfireBackgroundJobService(INotificationService notificationService)
    {
        _notificationService = notificationService;
    }

    public async Task ExecuteMorningJobAsync()
    {
        try
        {
            Console.WriteLine($"[{DateTime.Now}] Starting morning job execution...");
            
            // Gửi thông báo appointment
            await _notificationService.SendAppointmentRemindersAsync();
            
            // Gửi thông báo medication cho frequency = 2 (2 lần/ngày)
            await _notificationService.SendMedicationRemindersAsync(2);
            
            // Xử lý pending notifications
            await _notificationService.ProcessPendingNotificationsAsync();
            
            Console.WriteLine($"[{DateTime.Now}] Morning job completed successfully.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[{DateTime.Now}] Error in morning job: {ex.Message}");
            throw;
        }
    }

    public async Task ExecuteEveningJobAsync()
    {
        try
        {
            Console.WriteLine($"[{DateTime.Now}] Starting evening job execution...");
            
            // Gửi thông báo appointment
            await _notificationService.SendAppointmentRemindersAsync();
            
            // Gửi thông báo medication cho frequency = 1 (1 lần/ngày)
            await _notificationService.SendMedicationRemindersAsync(1);
            
            // Gửi thông báo medication cho frequency = 2 (2 lần/ngày) - lần thứ 2 trong ngày
            await _notificationService.SendMedicationRemindersAsync(2);
            
            // Xử lý pending notifications
            await _notificationService.ProcessPendingNotificationsAsync();
            
            Console.WriteLine($"[{DateTime.Now}] Evening job completed successfully.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[{DateTime.Now}] Error in evening job: {ex.Message}");
            throw;
        }
    }
}