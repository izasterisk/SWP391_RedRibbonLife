using BLL.DTO.Notification;
using BLL.Interfaces;
using BLL.Utils;

namespace BLL.Services;

public class NotificationService : INotificationService
{
    private readonly INotificationUtils _notificationUtils;
    private readonly SendGridEmailUtil _emailUtil;

    public NotificationService(INotificationUtils notificationUtils, SendGridEmailUtil emailUtil)
    {
        _notificationUtils = notificationUtils;
        _emailUtil = emailUtil;
    }

    public async Task SendAppointmentRemindersAsync()
    {
        try
        {
            var upcomingAppointments = await _notificationUtils.GetUpcomingAppointmentsAsync();
            foreach (var appointment in upcomingAppointments)
            {
                if (appointment.Patient?.User != null)
                {
                    var notificationCreateDTO = new NotificationCreateDTO
                    {
                        UserId = appointment.Patient.User.UserId,
                        AppointmentId = appointment.AppointmentId,
                        TreatmentId = null,
                        NotificationType = "Appointment",
                        ScheduledTime = DateTime.Now
                    };
                    var notification = await _notificationUtils.CreateNotificationAsync(notificationCreateDTO);
                    try
                    {
                        var appointmentDateTime = appointment.AppointmentDate.ToDateTime(appointment.AppointmentTime);
                        await _emailUtil.SendAppointmentReminderEmailAsync(
                            appointment.Patient.User.Email,
                            appointment.Patient.User.FullName ?? "Bệnh nhân",
                            appointmentDateTime
                        );
                        await _notificationUtils.MarkNotificationAsSentAsync(notification.NotificationId);
                    }
                    catch (Exception ex)
                    {
                        await _notificationUtils.MarkNotificationAsFailedAsync(notification.NotificationId, ex.Message);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in SendAppointmentRemindersAsync: {ex.Message}");
            throw;
        }
    }

    public async Task SendMedicationRemindersAsync(int frequency)
    {
        try
        {
            var activeTreatments = await _notificationUtils.GetActiveTreatmentsForMedicationReminder(frequency);
            
            foreach (var treatment in activeTreatments)
            {
                if (treatment.TestResult?.Patient?.User != null)
                {
                    // Tạo notification record
                    var notificationCreateDTO = new NotificationCreateDTO
                    {
                        UserId = treatment.TestResult.Patient.User.UserId,
                        AppointmentId = null,
                        TreatmentId = treatment.TreatmentId,
                        NotificationType = "Medication",
                        ScheduledTime = DateTime.Now
                    };
                    var notification = await _notificationUtils.CreateNotificationAsync(notificationCreateDTO);
                    try
                    {
                        // Gửi email
                        await _emailUtil.SendMedicationReminderEmailAsync(
                            treatment.TestResult.Patient.User.Email,
                            treatment.TestResult.Patient.User.FullName ?? "Bệnh nhân",
                            treatment.Regimen.RegimenName ?? "Phác đồ điều trị",
                            treatment.Regimen.UsageInstructions ?? "Theo chỉ dẫn của bác sĩ",
                            treatment.Regimen.Frequency
                        );
                        await _notificationUtils.MarkNotificationAsSentAsync(notification.NotificationId);
                    }
                    catch (Exception ex)
                    {
                        await _notificationUtils.MarkNotificationAsFailedAsync(notification.NotificationId, ex.Message);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in SendMedicationRemindersAsync: {ex.Message}");
            throw;
        }
    }

    public async Task ProcessPendingNotificationsAsync()
    {
        try
        {
            // Xử lý các notification bị treo
            var pendingNotifications = await _notificationUtils.GetPendingNotificationsAsync();
            foreach (var notification in pendingNotifications)
            {
                try
                {
                    bool retrySuccess = false;
                    if (notification.NotificationType == "Appointment" && notification.AppointmentId.HasValue)
                    {
                        // Retry appointment notification
                        var specificAppointment = await _notificationUtils.GetAppointmentByIdAsync(notification.AppointmentId.Value);
                        if (specificAppointment?.Patient?.User != null)
                        {
                            var appointmentDateTime = specificAppointment.AppointmentDate.ToDateTime(specificAppointment.AppointmentTime);
                            await _emailUtil.SendAppointmentReminderEmailAsync(
                                specificAppointment.Patient.User.Email,
                                specificAppointment.Patient.User.FullName ?? "Bệnh nhân",
                                appointmentDateTime
                            );
                            retrySuccess = true;
                        }
                    }
                    else if (notification.NotificationType == "Medication" && notification.TreatmentId.HasValue)
                    {
                        // Retry medication notification
                        var specificTreatment = await _notificationUtils.GetTreatmentByIdAsync(notification.TreatmentId.Value);
                        if (specificTreatment?.TestResult?.Patient?.User != null)
                        {
                            await _emailUtil.SendMedicationReminderEmailAsync(
                                specificTreatment.TestResult.Patient.User.Email,
                                specificTreatment.TestResult.Patient.User.FullName ?? "Bệnh nhân",
                                specificTreatment.Regimen.RegimenName ?? "Phác đồ điều trị",
                                specificTreatment.Regimen.UsageInstructions ?? "Theo chỉ dẫn của bác sĩ",
                                specificTreatment.Regimen.Frequency
                            );
                            retrySuccess = true;
                        }
                    }
                    if (retrySuccess)
                    {
                        // Đánh dấu thành công nếu gửi được
                        await _notificationUtils.MarkNotificationAsSentAsync(notification.NotificationId);
                    }
                    else
                    {
                        // Nếu không thể gửi (do không tìm thấy data), đánh dấu thất bại
                        string errorMessage = "Không thể tìm thấy dữ liệu liên quan để gửi notification";
                        await _notificationUtils.MarkNotificationAsFailedAsync(notification.NotificationId, errorMessage);
                        // Cập nhật error message thông qua UpdateErrorMessageAsync
                        var updateDTO = new NotificationUpdateDTO
                        {
                            NotificationId = notification.NotificationId,
                            ErrorMessage = errorMessage
                        };
                        await _notificationUtils.UpdateErrorMessageAsync(updateDTO);
                    }
                }
                catch (Exception ex)
                {
                    // Nếu có lỗi trong quá trình retry, đánh dấu thất bại và ghi lại error message
                    await _notificationUtils.MarkNotificationAsFailedAsync(notification.NotificationId, ex.Message);
                    // Cập nhật error message thông qua UpdateErrorMessageAsync
                    var updateDTO = new NotificationUpdateDTO
                    {
                        NotificationId = notification.NotificationId,
                        ErrorMessage = $"Retry failed: {ex.Message}"
                    };
                    await _notificationUtils.UpdateErrorMessageAsync(updateDTO);
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in ProcessPendingNotificationsAsync: {ex.Message}");
            throw;
        }
    }

    // public async Task ProcessFailedNotificationsRetryAsync()
    // {
    //     // Implement retry logic for failed notifications
    //     // This can be called separately or as part of the main jobs
    // }
}