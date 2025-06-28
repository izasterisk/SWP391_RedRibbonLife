using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DAL.Models;

[Index("NotificationType", Name = "IX_Notifications_NotificationType")]
[Index("Status", "ScheduledTime", Name = "IX_Notifications_Status_ScheduledTime")]
[Index("UserId", Name = "IX_Notifications_UserId")]
public partial class Notification
{
    [Key]
    [Column("notification_id")]
    public int NotificationId { get; set; }

    [Column("user_id")]
    public int UserId { get; set; }

    [Column("appointment_id")]
    public int? AppointmentId { get; set; }

    [Column("treatment_id")]
    public int? TreatmentId { get; set; }

    [Column("notification_type")]
    [StringLength(50)]
    public string NotificationType { get; set; } = null!;

    [Column("scheduled_time", TypeName = "datetime")]
    public DateTime ScheduledTime { get; set; }

    [Column("status")]
    [StringLength(50)]
    public string? Status { get; set; }

    [Column("sent_at", TypeName = "datetime")]
    public DateTime? SentAt { get; set; }

    [Column("retry_count")]
    public int? RetryCount { get; set; }

    [Column("error_message")]
    public string? ErrorMessage { get; set; }

    [ForeignKey("AppointmentId")]
    [InverseProperty("Notifications")]
    public virtual Appointment? Appointment { get; set; }

    [ForeignKey("TreatmentId")]
    [InverseProperty("Notifications")]
    public virtual Treatment? Treatment { get; set; }

    [ForeignKey("UserId")]
    [InverseProperty("Notifications")]
    public virtual User User { get; set; } = null!;
}
