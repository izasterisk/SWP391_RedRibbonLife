using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DAL.Models;

public partial class Patient
{
    [Key]
    [Column("patient_id")]
    public int PatientId { get; set; }

    [Column("user_id")]
    public int? UserId { get; set; }

    [Column("blood_type")]
    [StringLength(5)]
    [Unicode(false)]
    public string? BloodType { get; set; }

    [Column("is_pregnant")]
    public bool? IsPregnant { get; set; }

    [Column("special_notes")]
    public string? SpecialNotes { get; set; }

    [InverseProperty("Patient")]
    public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();

    [InverseProperty("Patient")]
    public virtual ICollection<MedicationSchedule> MedicationSchedules { get; set; } = new List<MedicationSchedule>();

    [InverseProperty("Patient")]
    public virtual ICollection<TestResult> TestResults { get; set; } = new List<TestResult>();

    [InverseProperty("Patient")]
    public virtual ICollection<TreatmentHistory> TreatmentHistories { get; set; } = new List<TreatmentHistory>();

    [ForeignKey("UserId")]
    [InverseProperty("Patients")]
    public virtual User? User { get; set; }
}
