using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DAL.Models;

[Table("Treatment")]
public partial class Treatment
{
    [Key]
    [Column("treatment_id")]
    public int TreatmentId { get; set; }

    [Column("test_result_id")]
    public int? TestResultId { get; set; }

    [Column("start_date")]
    public DateOnly StartDate { get; set; }

    [Column("end_date")]
    public DateOnly? EndDate { get; set; }

    [Column("status")]
    [StringLength(50)]
    public string? Status { get; set; }

    [Column("notes")]
    public string? Notes { get; set; }

    [InverseProperty("Treatment")]
    public virtual ICollection<Prescription> Prescriptions { get; set; } = new List<Prescription>();

    [ForeignKey("TestResultId")]
    [InverseProperty("Treatments")]
    public virtual TestResult? TestResult { get; set; }
}
