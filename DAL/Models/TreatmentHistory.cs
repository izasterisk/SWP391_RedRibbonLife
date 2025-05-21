using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DAL.Models;

public partial class TreatmentHistory
{
    [Key]
    [Column("treatment_id")]
    public int TreatmentId { get; set; }

    [Column("prescription_id")]
    public int? PrescriptionId { get; set; }

    [Column("patient_id")]
    public int? PatientId { get; set; }

    [Column("doctor_id")]
    public int? DoctorId { get; set; }

    [Column("start_date")]
    public DateOnly StartDate { get; set; }

    [Column("end_date")]
    public DateOnly? EndDate { get; set; }

    [Column("status")]
    [StringLength(50)]
    public string? Status { get; set; }

    [Column("notes")]
    public string? Notes { get; set; }

    [ForeignKey("DoctorId")]
    [InverseProperty("TreatmentHistories")]
    public virtual Doctor? Doctor { get; set; }

    [ForeignKey("PatientId")]
    [InverseProperty("TreatmentHistories")]
    public virtual Patient? Patient { get; set; }

    [InverseProperty("Treatment")]
    public virtual ICollection<Prescription> Prescriptions { get; set; } = new List<Prescription>();
}
