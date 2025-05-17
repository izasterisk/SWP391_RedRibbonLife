using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DAL.Models;

public partial class Appointment
{
    [Key]
    [Column("AppointmentID")]
    public int AppointmentId { get; set; }

    [StringLength(20)]
    [Unicode(false)]
    public string PatientCode { get; set; } = null!;

    [StringLength(20)]
    [Unicode(false)]
    public string DoctorCode { get; set; } = null!;

    public DateOnly AppointmentDate { get; set; }

    public TimeOnly AppointmentTime { get; set; }

    [StringLength(200)]
    public string? Purpose { get; set; }

    [StringLength(20)]
    public string? Status { get; set; }

    public string? Notes { get; set; }

    public bool? IsOnline { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? CreatedDate { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? ModifiedDate { get; set; }

    [InverseProperty("Appointment")]
    public virtual ICollection<AppointmentReminder> AppointmentReminders { get; set; } = new List<AppointmentReminder>();

    [ForeignKey("DoctorCode")]
    [InverseProperty("Appointments")]
    public virtual Doctor DoctorCodeNavigation { get; set; } = null!;

    [ForeignKey("PatientCode")]
    [InverseProperty("Appointments")]
    public virtual Patient PatientCodeNavigation { get; set; } = null!;

    [InverseProperty("Appointment")]
    public virtual ICollection<TestResult> TestResults { get; set; } = new List<TestResult>();
}
