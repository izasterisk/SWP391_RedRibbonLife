using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DAL.Models;

public partial class Appointment
{
    [Key]
    [Column("appointment_id")]
    public int AppointmentId { get; set; }

    [Column("patient_id")]
    public int? PatientId { get; set; }

    [Column("doctor_id")]
    public int DoctorId { get; set; }

    [Column("appointment_date")]
    public DateOnly AppointmentDate { get; set; }

    [Column("appointment_time")]
    public TimeOnly AppointmentTime { get; set; }

    [Column("appointment_type")]
    [StringLength(50)]
    public string? AppointmentType { get; set; }

    [Column("status")]
    [StringLength(50)]
    public string? Status { get; set; }

    [Column("isAnonymous")]
    public bool? IsAnonymous { get; set; }

    [ForeignKey("DoctorId")]
    [InverseProperty("Appointments")]
    public virtual Doctor Doctor { get; set; } = null!;

    [InverseProperty("Appointment")]
    public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();

    [ForeignKey("PatientId")]
    [InverseProperty("Appointments")]
    public virtual Patient? Patient { get; set; }

    [InverseProperty("Appointment")]
    public virtual TestResult? TestResult { get; set; }
}
