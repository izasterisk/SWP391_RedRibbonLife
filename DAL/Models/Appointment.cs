﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DAL.Models;

[Index("AppointmentDate", "AppointmentTime", "DoctorId", Name = "IX_Appointments_Date_Time_Doctor")]
[Index("DoctorId", Name = "IX_Appointments_DoctorId")]
[Index("PatientId", Name = "IX_Appointments_PatientId")]
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

    [Column("test_type_id")]
    public int? TestTypeId { get; set; }

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
    public virtual ICollection<TestResult> TestResults { get; set; } = new List<TestResult>();

    [ForeignKey("TestTypeId")]
    [InverseProperty("Appointments")]
    public virtual TestType? TestType { get; set; }
}
