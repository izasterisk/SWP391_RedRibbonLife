using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DAL.Models;

[Index("Username", Name = "UQ__Users__536C85E4A2B30F59", IsUnique = true)]
public partial class User
{
    [Key]
    [Column("UserID")]
    public int UserId { get; set; }

    [Column("RoleID")]
    public int? RoleId { get; set; }

    [StringLength(50)]
    public string Username { get; set; } = null!;

    [StringLength(255)]
    public string Password { get; set; } = null!;

    [StringLength(100)]
    public string FullName { get; set; } = null!;

    public DateOnly? DateOfBirth { get; set; }

    [StringLength(10)]
    public string? Gender { get; set; }

    [StringLength(200)]
    public string? Address { get; set; }

    [StringLength(20)]
    [Unicode(false)]
    public string? Phone { get; set; }

    [StringLength(100)]
    [Unicode(false)]
    public string? Email { get; set; }

    public bool? IsAnonymous { get; set; }

    public bool? IsActive { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? CreatedDate { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? ModifiedDate { get; set; }

    [InverseProperty("AuthorNavigation")]
    public virtual ICollection<BlogPost> BlogPosts { get; set; } = new List<BlogPost>();

    [InverseProperty("User")]
    public virtual ICollection<Doctor> Doctors { get; set; } = new List<Doctor>();

    [InverseProperty("AuthorNavigation")]
    public virtual ICollection<EducationalMaterial> EducationalMaterials { get; set; } = new List<EducationalMaterial>();

    [InverseProperty("User")]
    public virtual ICollection<Patient> Patients { get; set; } = new List<Patient>();

    [ForeignKey("RoleId")]
    [InverseProperty("Users")]
    public virtual Role? Role { get; set; }

    [InverseProperty("PerformedByNavigation")]
    public virtual ICollection<TestResult> TestResults { get; set; } = new List<TestResult>();
}
