using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DAL.Models;

public partial class Article
{
    [Key]
    [Column("article_id")]
    public int ArticleId { get; set; }

    [Column("title")]
    [StringLength(200)]
    public string Title { get; set; } = null!;

    [Column("content")]
    public string? Content { get; set; }

    [Column("category_id")]
    public int? CategoryId { get; set; }

    [Column("isActive")]
    public bool? IsActive { get; set; }

    [Column("createdDate")]
    public DateOnly CreatedDate { get; set; }

    [Column("author")]
    [StringLength(100)]
    public string? Author { get; set; }

    [ForeignKey("CategoryId")]
    [InverseProperty("Articles")]
    public virtual Category? Category { get; set; }
}
