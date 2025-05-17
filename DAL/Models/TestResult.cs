using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DAL.Models;

public partial class TestResult
{
    [Key]
    [Column("TestID")]
    public int TestId { get; set; }

    [StringLength(20)]
    [Unicode(false)]
    public string PatientCode { get; set; } = null!;

    [Column("AppointmentID")]
    public int? AppointmentId { get; set; }

    [StringLength(50)]
    public string TestType { get; set; } = null!;

    public DateOnly TestDate { get; set; }

    [StringLength(100)]
    public string? Results { get; set; }

    [StringLength(20)]
    public string? Units { get; set; }

    [StringLength(50)]
    public string? NormalRange { get; set; }

    public string? Interpretation { get; set; }

    public int? PerformedBy { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? CreatedDate { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? ModifiedDate { get; set; }

    [ForeignKey("AppointmentId")]
    [InverseProperty("TestResults")]
    public virtual Appointment? Appointment { get; set; }

    [ForeignKey("PatientCode")]
    [InverseProperty("TestResults")]
    public virtual Patient PatientCodeNavigation { get; set; } = null!;

    [ForeignKey("PerformedBy")]
    [InverseProperty("TestResults")]
    public virtual User? PerformedByNavigation { get; set; }
}
