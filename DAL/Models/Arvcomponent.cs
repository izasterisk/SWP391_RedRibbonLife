using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DAL.Models;

[Table("ARVComponents")]
[Index("ComponentName", Name = "UQ__ARVCompo__2E7CCD4B5CAF831E", IsUnique = true)]
public partial class Arvcomponent
{
    [Key]
    [Column("component_id")]
    public int ComponentId { get; set; }

    [Column("component_name")]
    [StringLength(100)]
    [Unicode(false)]
    public string ComponentName { get; set; } = null!;

    [Column("description")]
    public string? Description { get; set; }

    [InverseProperty("Component1")]
    public virtual ICollection<Arvregimen> ArvregimenComponent1s { get; set; } = new List<Arvregimen>();

    [InverseProperty("Component2")]
    public virtual ICollection<Arvregimen> ArvregimenComponent2s { get; set; } = new List<Arvregimen>();

    [InverseProperty("Component3")]
    public virtual ICollection<Arvregimen> ArvregimenComponent3s { get; set; } = new List<Arvregimen>();

    [InverseProperty("Component4")]
    public virtual ICollection<Arvregimen> ArvregimenComponent4s { get; set; } = new List<Arvregimen>();
}
