﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DAL.Models;

public partial class Prescription
{
    [Key]
    [Column("prescription_id")]
    public int PrescriptionId { get; set; }

    [Column("treatment_id")]
    public int? TreatmentId { get; set; }

    [Column("regimen_id")]
    public int RegimenId { get; set; }

    [InverseProperty("Prescription")]
    public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();

    [ForeignKey("RegimenId")]
    [InverseProperty("Prescriptions")]
    public virtual Arvregimen Regimen { get; set; } = null!;

    [ForeignKey("TreatmentId")]
    [InverseProperty("Prescriptions")]
    public virtual Treatment? Treatment { get; set; }
}
