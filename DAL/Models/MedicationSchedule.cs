using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DAL.Models;

public partial class MedicationSchedule
{
    [Key]
    [Column("schedule_id")]
    public int ScheduleId { get; set; }

    [Column("prescription_id")]
    public int? PrescriptionId { get; set; }

    [Column("patient_id")]
    public int? PatientId { get; set; }

    [Column("medication_time")]
    public int MedicationTime { get; set; }

    [Column("sent_at", TypeName = "datetime")]
    public DateTime? SentAt { get; set; }

    [ForeignKey("PatientId")]
    [InverseProperty("MedicationSchedules")]
    public virtual Patient? Patient { get; set; }

    [ForeignKey("PrescriptionId")]
    [InverseProperty("MedicationSchedules")]
    public virtual Prescription? Prescription { get; set; }
}
