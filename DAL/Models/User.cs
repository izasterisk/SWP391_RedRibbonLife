using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DAL.Models;

[Index("Email", "IsActive", Name = "IX_Users_Email_IsActive")]
[Index("FullName", "IsActive", Name = "IX_Users_FullName_IsActive")]
[Index("UserRole", "IsActive", Name = "IX_Users_UserRole_IsActive")]
[Index("Username", "IsActive", "IsVerified", Name = "IX_Users_Username_IsActive_IsVerified")]
[Index("Email", Name = "UQ__Users__AB6E61640EAA6F86", IsUnique = true)]
[Index("Username", Name = "UQ__Users__F3DBC572810DCE6C", IsUnique = true)]
public partial class User
{
    [Key]
    [Column("user_id")]
    public int UserId { get; set; }

    [Column("username")]
    [StringLength(50)]
    [Unicode(false)]
    public string Username { get; set; } = null!;

    [Column("password")]
    [StringLength(255)]
    public string Password { get; set; } = null!;

    [Column("email")]
    [StringLength(100)]
    public string Email { get; set; } = null!;

    [Column("phone_number")]
    [StringLength(20)]
    [Unicode(false)]
    public string? PhoneNumber { get; set; }

    [Column("full_name")]
    [StringLength(100)]
    public string? FullName { get; set; }

    [Column("date_of_birth")]
    public DateOnly? DateOfBirth { get; set; }

    [Column("gender")]
    [StringLength(10)]
    public string? Gender { get; set; }

    [Column("address")]
    public string? Address { get; set; }

    [Column("user_role")]
    [StringLength(50)]
    public string UserRole { get; set; } = null!;

    [Column("isActive")]
    public bool IsActive { get; set; }

    [Column("isVerified")]
    public bool IsVerified { get; set; }

    [InverseProperty("User")]
    public virtual ICollection<Article> Articles { get; set; } = new List<Article>();

    [InverseProperty("User")]
    public virtual ICollection<Doctor> Doctors { get; set; } = new List<Doctor>();

    [InverseProperty("User")]
    public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();

    [InverseProperty("User")]
    public virtual ICollection<Patient> Patients { get; set; } = new List<Patient>();
}
