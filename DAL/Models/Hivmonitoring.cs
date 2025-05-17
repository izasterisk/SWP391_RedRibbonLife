using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DAL.Models;

[Table("HIVMonitoring")]
public partial class Hivmonitoring
{
    [Key]
    [Column("MonitoringID")]
    public int MonitoringId { get; set; }

    [StringLength(20)]
    [Unicode(false)]
    public string PatientCode { get; set; } = null!;

    public DateOnly TestDate { get; set; }

    [Column("CD4Count")]
    public int? Cd4count { get; set; }

    [Column(TypeName = "decimal(10, 2)")]
    public decimal? ViralLoad { get; set; }

    public string? Notes { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? CreatedDate { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? ModifiedDate { get; set; }

    [ForeignKey("PatientCode")]
    [InverseProperty("Hivmonitorings")]
    public virtual Patient PatientCodeNavigation { get; set; } = null!;
}
