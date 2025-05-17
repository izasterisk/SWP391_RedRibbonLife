using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DAL.Models;

public partial class EducationalMaterial
{
    [Key]
    [Column("MaterialID")]
    public int MaterialId { get; set; }

    [StringLength(200)]
    public string Title { get; set; } = null!;

    public string? Content { get; set; }

    [StringLength(50)]
    public string? Category { get; set; }

    public DateOnly? PublishedDate { get; set; }

    public int? Author { get; set; }

    public bool? IsPublished { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? CreatedDate { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? ModifiedDate { get; set; }

    [ForeignKey("Author")]
    [InverseProperty("EducationalMaterials")]
    public virtual User? AuthorNavigation { get; set; }
}
