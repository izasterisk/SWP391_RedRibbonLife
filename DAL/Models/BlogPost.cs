using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DAL.Models;

public partial class BlogPost
{
    [Key]
    [Column("PostID")]
    public int PostId { get; set; }

    [StringLength(200)]
    public string Title { get; set; } = null!;

    public string? Content { get; set; }

    public int Author { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? PublishedDate { get; set; }

    public bool? IsPublished { get; set; }

    public int? ViewCount { get; set; }

    [StringLength(200)]
    public string? Tags { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? CreatedDate { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? ModifiedDate { get; set; }

    [ForeignKey("Author")]
    [InverseProperty("BlogPosts")]
    public virtual User AuthorNavigation { get; set; } = null!;
}
