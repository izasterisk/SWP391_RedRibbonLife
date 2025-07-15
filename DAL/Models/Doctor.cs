using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DAL.Models;

[Index("UserId", Name = "IX_Doctors_UserId")]
public partial class Doctor
{
    [Key]
    [Column("doctor_id")]
    public int DoctorId { get; set; }

    [Column("user_id")]
    public int UserId { get; set; }

    [Column("bio")]
    public string? Bio { get; set; }

    [InverseProperty("Doctor")]
    public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();

    [InverseProperty("Doctor")]
    public virtual ICollection<DoctorCertificate> DoctorCertificates { get; set; } = new List<DoctorCertificate>();

    [InverseProperty("Doctor")]
    public virtual ICollection<DoctorSchedule> DoctorSchedules { get; set; } = new List<DoctorSchedule>();

    [InverseProperty("Doctor")]
    public virtual ICollection<TestResult> TestResults { get; set; } = new List<TestResult>();

    [ForeignKey("UserId")]
    [InverseProperty("Doctors")]
    public virtual User User { get; set; } = null!;
}
