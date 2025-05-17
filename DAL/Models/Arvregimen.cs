using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DAL.Models;

[Table("ARVRegimens")]
public partial class Arvregimen
{
    [Key]
    [Column("RegimenID")]
    public int RegimenId { get; set; }

    [StringLength(100)]
    public string RegimenName { get; set; } = null!;

    [StringLength(500)]
    public string? Description { get; set; }

    [StringLength(255)]
    public string? Medications { get; set; }

    [StringLength(200)]
    public string? UseCase { get; set; }

    [StringLength(500)]
    public string? SideEffects { get; set; }

    public string? Recommendations { get; set; }

    public bool? IsActive { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? CreatedDate { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? ModifiedDate { get; set; }

    [InverseProperty("Regimen")]
    public virtual ICollection<Hivtreatment> Hivtreatments { get; set; } = new List<Hivtreatment>();
}
