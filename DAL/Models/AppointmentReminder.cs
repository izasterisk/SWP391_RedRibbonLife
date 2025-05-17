using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DAL.Models;

public partial class AppointmentReminder
{
    [Key]
    [Column("ReminderID")]
    public int ReminderId { get; set; }

    [Column("AppointmentID")]
    public int AppointmentId { get; set; }

    [StringLength(20)]
    [Unicode(false)]
    public string PatientCode { get; set; } = null!;

    public DateOnly ReminderDate { get; set; }

    public bool? IsSent { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? SentDate { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? CreatedDate { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? ModifiedDate { get; set; }

    [ForeignKey("AppointmentId")]
    [InverseProperty("AppointmentReminders")]
    public virtual Appointment Appointment { get; set; } = null!;

    [ForeignKey("PatientCode")]
    [InverseProperty("AppointmentReminders")]
    public virtual Patient PatientCodeNavigation { get; set; } = null!;
}
