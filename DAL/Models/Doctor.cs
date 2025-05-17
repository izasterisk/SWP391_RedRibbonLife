using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DAL.Models;

public partial class Doctor
{
    [Key]
    [StringLength(20)]
    [Unicode(false)]
    public string DoctorCode { get; set; } = null!;

    [Column("UserID")]
    public int UserId { get; set; }

    [StringLength(100)]
    public string? Specialization { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string? LicenseNumber { get; set; }

    [StringLength(200)]
    public string? Qualification { get; set; }

    public int? Experience { get; set; }

    public string? Biography { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? CreatedDate { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? ModifiedDate { get; set; }

    [InverseProperty("DoctorCodeNavigation")]
    public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();

    [InverseProperty("DoctorCodeNavigation")]
    public virtual ICollection<DoctorSchedule> DoctorSchedules { get; set; } = new List<DoctorSchedule>();

    [InverseProperty("DoctorCodeNavigation")]
    public virtual ICollection<Hivtreatment> Hivtreatments { get; set; } = new List<Hivtreatment>();

    [ForeignKey("UserId")]
    [InverseProperty("Doctors")]
    public virtual User User { get; set; } = null!;
}
