using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DAL.Models;

[Table("ARVRegimens")]
[Index("IsActive", Name = "IX_ARVRegimens_IsActive")]
[Index("IsCustomized", "IsActive", Name = "IX_ARVRegimens_IsCustomized_IsActive")]
public partial class Arvregimen
{
    [Key]
    [Column("regimen_id")]
    public int RegimenId { get; set; }

    [Column("regimen_name")]
    [StringLength(100)]
    public string? RegimenName { get; set; }

    [Column("component1_id")]
    public int Component1Id { get; set; }

    [Column("component2_id")]
    public int? Component2Id { get; set; }

    [Column("component3_id")]
    public int? Component3Id { get; set; }

    [Column("component4_id")]
    public int? Component4Id { get; set; }

    [Column("description")]
    public string? Description { get; set; }

    [Column("suitable_for")]
    public string? SuitableFor { get; set; }

    [Column("side_effects")]
    public string? SideEffects { get; set; }

    [Column("usage_instructions")]
    public string? UsageInstructions { get; set; }

    [Column("frequency")]
    public int Frequency { get; set; }

    [Column("isActive")]
    public bool IsActive { get; set; }

    [Column("isCustomized")]
    public bool IsCustomized { get; set; }

    [ForeignKey("Component1Id")]
    [InverseProperty("ArvregimenComponent1s")]
    public virtual Arvcomponent Component1 { get; set; } = null!;

    [ForeignKey("Component2Id")]
    [InverseProperty("ArvregimenComponent2s")]
    public virtual Arvcomponent? Component2 { get; set; }

    [ForeignKey("Component3Id")]
    [InverseProperty("ArvregimenComponent3s")]
    public virtual Arvcomponent? Component3 { get; set; }

    [ForeignKey("Component4Id")]
    [InverseProperty("ArvregimenComponent4s")]
    public virtual Arvcomponent? Component4 { get; set; }

    [InverseProperty("Regimen")]
    public virtual ICollection<Treatment> Treatments { get; set; } = new List<Treatment>();
}
