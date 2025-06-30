using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DAL.Models;

[Index("CategoryId", "IsActive", Name = "IX_Articles_CategoryId_IsActive")]
[Index("Title", Name = "IX_Articles_Title")]
[Index("UserId", "IsActive", Name = "IX_Articles_UserId_IsActive")]
public partial class Article
{
    [Key]
    [Column("article_id")]
    public int ArticleId { get; set; }

    [Column("title")]
    [StringLength(200)]
    public string Title { get; set; } = null!;

    [Column("content")]
    public string Content { get; set; } = null!;

    [Column("category_id")]
    public int? CategoryId { get; set; }

    [Column("isActive")]
    public bool? IsActive { get; set; }

    [Column("createdDate")]
    public DateOnly CreatedDate { get; set; }

    [Column("user_id")]
    public int? UserId { get; set; }

    [ForeignKey("CategoryId")]
    [InverseProperty("Articles")]
    public virtual Category? Category { get; set; }

    [ForeignKey("UserId")]
    [InverseProperty("Articles")]
    public virtual User? User { get; set; }
}
