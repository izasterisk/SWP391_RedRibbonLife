using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DAL.Models;

public partial class Reminder
{
    [Key]
    [Column("reminder_id")]
    public int ReminderId { get; set; }

    [Column("appointment_id")]
    public int? AppointmentId { get; set; }

    [Column("reminder_time", TypeName = "datetime")]
    public DateTime ReminderTime { get; set; }

    [Column("reminder_type")]
    [StringLength(50)]
    public string? ReminderType { get; set; }

    [Column("status")]
    [StringLength(50)]
    public string? Status { get; set; }

    [Column("sent_at", TypeName = "datetime")]
    public DateTime? SentAt { get; set; }

    [ForeignKey("AppointmentId")]
    [InverseProperty("Reminders")]
    public virtual Appointment? Appointment { get; set; }
}
