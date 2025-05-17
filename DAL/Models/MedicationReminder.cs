using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DAL.Models;

public partial class MedicationReminder
{
    [Key]
    [Column("ReminderID")]
    public int ReminderId { get; set; }

    [StringLength(20)]
    [Unicode(false)]
    public string PatientCode { get; set; } = null!;

    [Column("TreatmentID")]
    public int TreatmentId { get; set; }

    public TimeOnly ReminderTime { get; set; }

    [StringLength(50)]
    public string? Frequency { get; set; }

    public bool? IsActive { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? LastSent { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? CreatedDate { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? ModifiedDate { get; set; }

    [ForeignKey("PatientCode")]
    [InverseProperty("MedicationReminders")]
    public virtual Patient PatientCodeNavigation { get; set; } = null!;

    [ForeignKey("TreatmentId")]
    [InverseProperty("MedicationReminders")]
    public virtual Hivtreatment Treatment { get; set; } = null!;
}
