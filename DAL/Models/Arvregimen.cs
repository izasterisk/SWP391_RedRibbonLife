using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DAL.Models;

[Table("ARVRegimens")]
[Index("RegimenCode", Name = "UQ__ARVRegim__A554D5B9CDDB84E1", IsUnique = true)]
public partial class Arvregimen
{
    [Key]
    [Column("regimen_id")]
    public int RegimenId { get; set; }

    [Column("regimen_name")]
    [StringLength(100)]
    public string RegimenName { get; set; } = null!;

    [Column("regimen_code")]
    [StringLength(20)]
    public string? RegimenCode { get; set; }

    [Column("components")]
    public string Components { get; set; } = null!;

    [Column("description")]
    public string? Description { get; set; }

    [Column("suitable_for")]
    public string? SuitableFor { get; set; }

    [Column("side_effects")]
    public string? SideEffects { get; set; }

    [Column("usage_instructions")]
    public string? UsageInstructions { get; set; }

    [Column("isActive")]
    public bool? IsActive { get; set; }

    [InverseProperty("Regimen")]
    public virtual ICollection<Prescription> Prescriptions { get; set; } = new List<Prescription>();
}
