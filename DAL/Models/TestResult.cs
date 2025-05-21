using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DAL.Models;

public partial class TestResult
{
    [Key]
    [Column("test_result_id")]
    public int TestResultId { get; set; }

    [Column("appointment_id")]
    public int? AppointmentId { get; set; }

    [Column("patient_id")]
    public int PatientId { get; set; }

    [Column("doctor_id")]
    public int? DoctorId { get; set; }

    [Column("test_type")]
    [StringLength(100)]
    public string TestType { get; set; } = null!;

    [Column("result_value")]
    [StringLength(255)]
    public string? ResultValue { get; set; }

    [Column("unit")]
    [StringLength(50)]
    public string? Unit { get; set; }

    [Column("normal_range")]
    [StringLength(50)]
    public string? NormalRange { get; set; }

    [Column("notes")]
    public string? Notes { get; set; }

    [ForeignKey("AppointmentId")]
    [InverseProperty("TestResults")]
    public virtual Appointment? Appointment { get; set; }

    [ForeignKey("DoctorId")]
    [InverseProperty("TestResults")]
    public virtual Doctor? Doctor { get; set; }

    [ForeignKey("PatientId")]
    [InverseProperty("TestResults")]
    public virtual Patient Patient { get; set; } = null!;
}
