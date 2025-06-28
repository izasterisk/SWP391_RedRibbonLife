using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DAL.Models;

[Index("AppointmentId", Name = "IX_TestResults_AppointmentId")]
[Index("DoctorId", Name = "IX_TestResults_DoctorId")]
[Index("PatientId", Name = "IX_TestResults_PatientId")]
[Index("TestTypeId", Name = "IX_TestResults_TestTypeId")]
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
    public int DoctorId { get; set; }

    [Column("test_type_id")]
    public int TestTypeId { get; set; }

    [Column("result_value")]
    [StringLength(255)]
    public string? ResultValue { get; set; }

    [Column("notes")]
    public string? Notes { get; set; }

    [ForeignKey("AppointmentId")]
    [InverseProperty("TestResults")]
    public virtual Appointment? Appointment { get; set; }

    [ForeignKey("DoctorId")]
    [InverseProperty("TestResults")]
    public virtual Doctor Doctor { get; set; } = null!;

    [ForeignKey("PatientId")]
    [InverseProperty("TestResults")]
    public virtual Patient Patient { get; set; } = null!;

    [ForeignKey("TestTypeId")]
    [InverseProperty("TestResults")]
    public virtual TestType TestType { get; set; } = null!;

    [InverseProperty("TestResult")]
    public virtual ICollection<Treatment> Treatments { get; set; } = new List<Treatment>();
}
