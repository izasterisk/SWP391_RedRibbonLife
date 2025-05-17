using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DAL.Models;

public partial class Patient
{
    [Key]
    [StringLength(20)]
    [Unicode(false)]
    public string PatientCode { get; set; } = null!;

    [Column("UserID")]
    public int UserId { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string? InsuranceNumber { get; set; }

    [StringLength(100)]
    public string? EmergencyContact { get; set; }

    [StringLength(20)]
    [Unicode(false)]
    public string? EmergencyPhone { get; set; }

    [StringLength(5)]
    [Unicode(false)]
    public string? BloodType { get; set; }

    [Column(TypeName = "decimal(5, 2)")]
    public decimal? Weight { get; set; }

    [Column(TypeName = "decimal(5, 2)")]
    public decimal? Height { get; set; }

    [Column("IsHIVPositive")]
    public bool? IsHivpositive { get; set; }

    [Column("HIVDiagnosisDate")]
    public DateOnly? HivdiagnosisDate { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? CreatedDate { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? ModifiedDate { get; set; }

    [InverseProperty("PatientCodeNavigation")]
    public virtual ICollection<AppointmentReminder> AppointmentReminders { get; set; } = new List<AppointmentReminder>();

    [InverseProperty("PatientCodeNavigation")]
    public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();

    [InverseProperty("PatientCodeNavigation")]
    public virtual ICollection<Hivmonitoring> Hivmonitorings { get; set; } = new List<Hivmonitoring>();

    [InverseProperty("PatientCodeNavigation")]
    public virtual ICollection<Hivtreatment> Hivtreatments { get; set; } = new List<Hivtreatment>();

    [InverseProperty("PatientCodeNavigation")]
    public virtual ICollection<MedicationReminder> MedicationReminders { get; set; } = new List<MedicationReminder>();

    [InverseProperty("PatientCodeNavigation")]
    public virtual ICollection<TestResult> TestResults { get; set; } = new List<TestResult>();

    [ForeignKey("UserId")]
    [InverseProperty("Patients")]
    public virtual User User { get; set; } = null!;
}
