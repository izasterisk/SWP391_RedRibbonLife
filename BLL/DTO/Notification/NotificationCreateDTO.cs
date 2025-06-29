using System.ComponentModel.DataAnnotations;

namespace BLL.DTO.Notification;

public class NotificationCreateDTO
{
    // public int NotificationId { get; set; }

    [Required(ErrorMessage = "User ID is required")]
    [Range(1, int.MaxValue, ErrorMessage = "User ID must be a positive number")]
    public int UserId { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Appointment ID must be a positive number")]
    public int? AppointmentId { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Treatment ID must be a positive number")]
    public int? TreatmentId { get; set; }

    [Required(ErrorMessage = "Notification type is required")]
    [StringLength(50, ErrorMessage = "Notification type cannot exceed 50 characters")]
    [AllowedValues("Appointment", "Medication", "General", ErrorMessage = "Notification type must be one of: Appointment, Medication, General")]
    public string NotificationType { get; set; } = null!;

    [Required(ErrorMessage = "Scheduled time is required")]
    public DateTime ScheduledTime { get; set; }

    // [StringLength(50, ErrorMessage = "Status cannot exceed 50 characters")]
    // [AllowedValues("Pending", "Sent", "Failed", "Cancelled", ErrorMessage = "Status must be one of: Pending, Sent, Failed, Cancelled")]
    // public string? Status { get; set; } = "Pending";

    // public DateTime? SentAt { get; set; }

    // [Range(0, int.MaxValue, ErrorMessage = "Retry count must be a non-negative number")]
    // public int? RetryCount { get; set; } = 0;

    // public string? ErrorMessage { get; set; }
}