using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DAL.Models;

public partial class DoctorSchedule
{
    [Key]
    [Column("ScheduleID")]
    public int ScheduleId { get; set; }

    [StringLength(20)]
    [Unicode(false)]
    public string DoctorCode { get; set; } = null!;

    public int? WeekDay { get; set; }

    public TimeOnly? StartTime { get; set; }

    public TimeOnly? EndTime { get; set; }

    public int? MaxAppointments { get; set; }

    public bool? IsAvailable { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? CreatedDate { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? ModifiedDate { get; set; }

    [ForeignKey("DoctorCode")]
    [InverseProperty("DoctorSchedules")]
    public virtual Doctor DoctorCodeNavigation { get; set; } = null!;
}
