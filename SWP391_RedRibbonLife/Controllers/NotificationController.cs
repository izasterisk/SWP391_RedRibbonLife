using BLL.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace SWP391_RedRibbonLife.Controllers;

[Route("api/[controller]")]
[ApiController]
public class NotificationController : ControllerBase
{
    private readonly INotificationService _notificationService;
    private readonly IHangfireBackgroundJobService _backgroundJobService;

    public NotificationController(
        INotificationService notificationService,
        IHangfireBackgroundJobService backgroundJobService)
    {
        _notificationService = notificationService;
        _backgroundJobService = backgroundJobService;
    }

    [HttpPost("test-morning-job")]
    [Authorize(AuthenticationSchemes = "LoginforLocaluser")]
    public async Task<IActionResult> TestMorningJob()
    {
        try
        {
            await _backgroundJobService.ExecuteMorningJobAsync();
            return Ok(new { message = "Morning job executed successfully" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPost("test-evening-job")]
    [Authorize(AuthenticationSchemes = "LoginforLocaluser")]
    public async Task<IActionResult> TestEveningJob()
    {
        try
        {
            await _backgroundJobService.ExecuteEveningJobAsync();
            return Ok(new { message = "Evening job executed successfully" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPost("send-appointment-reminders")]
    [Authorize(AuthenticationSchemes = "LoginforLocaluser")]
    public async Task<IActionResult> SendAppointmentReminders()
    {
        try
        {
            await _notificationService.SendAppointmentRemindersAsync();
            return Ok(new { message = "Appointment reminders sent successfully" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPost("send-medication-reminders/{frequency}")]
    [Authorize(AuthenticationSchemes = "LoginforLocaluser")]
    public async Task<IActionResult> SendMedicationReminders(int frequency)
    {
        try
        {
            if (frequency != 1 && frequency != 2)
            {
                return BadRequest(new { error = "Frequency must be 1 or 2" });
            }

            await _notificationService.SendMedicationRemindersAsync(frequency);
            return Ok(new { message = $"Medication reminders (frequency {frequency}) sent successfully" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
}