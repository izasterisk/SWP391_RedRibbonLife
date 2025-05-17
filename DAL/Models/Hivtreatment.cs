using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DAL.Models;

[Table("HIVTreatments")]
public partial class Hivtreatment
{
    [Key]
    [Column("TreatmentID")]
    public int TreatmentId { get; set; }

    [StringLength(20)]
    [Unicode(false)]
    public string PatientCode { get; set; } = null!;

    [StringLength(20)]
    [Unicode(false)]
    public string DoctorCode { get; set; } = null!;

    [Column("RegimenID")]
    public int RegimenId { get; set; }

    public DateOnly StartDate { get; set; }

    public DateOnly? EndDate { get; set; }

    [StringLength(20)]
    public string? Status { get; set; }

    [StringLength(100)]
    public string? Dosage { get; set; }

    [StringLength(50)]
    public string? Frequency { get; set; }

    [StringLength(500)]
    public string? SideEffectsObserved { get; set; }

    public string? Notes { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? CreatedDate { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? ModifiedDate { get; set; }

    [ForeignKey("DoctorCode")]
    [InverseProperty("Hivtreatments")]
    public virtual Doctor DoctorCodeNavigation { get; set; } = null!;

    [InverseProperty("Treatment")]
    public virtual ICollection<MedicationReminder> MedicationReminders { get; set; } = new List<MedicationReminder>();

    [ForeignKey("PatientCode")]
    [InverseProperty("Hivtreatments")]
    public virtual Patient PatientCodeNavigation { get; set; } = null!;

    [ForeignKey("RegimenId")]
    [InverseProperty("Hivtreatments")]
    public virtual Arvregimen Regimen { get; set; } = null!;
}
