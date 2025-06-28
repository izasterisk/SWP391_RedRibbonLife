using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DAL.Models;

[Table("Treatment")]
[Index("RegimenId", Name = "IX_Treatment_RegimenId")]
[Index("Status", Name = "IX_Treatment_Status")]
[Index("TestResultId", Name = "IX_Treatment_TestResultId")]
public partial class Treatment
{
    [Key]
    [Column("treatment_id")]
    public int TreatmentId { get; set; }

    [Column("test_result_id")]
    public int? TestResultId { get; set; }

    [Column("regimen_id")]
    public int RegimenId { get; set; }

    [Column("start_date")]
    public DateOnly StartDate { get; set; }

    [Column("end_date")]
    public DateOnly EndDate { get; set; }

    [Column("status")]
    [StringLength(50)]
    public string? Status { get; set; }

    [Column("notes")]
    public string? Notes { get; set; }

    [InverseProperty("Treatment")]
    public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();

    [ForeignKey("RegimenId")]
    [InverseProperty("Treatments")]
    public virtual Arvregimen Regimen { get; set; } = null!;

    [ForeignKey("TestResultId")]
    [InverseProperty("Treatments")]
    public virtual TestResult? TestResult { get; set; }
}
