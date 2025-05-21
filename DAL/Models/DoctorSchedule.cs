using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DAL.Models;

public partial class DoctorSchedule
{
    [Key]
    [Column("schedule_id")]
    public int ScheduleId { get; set; }

    [Column("doctor_id")]
    public int? DoctorId { get; set; }

    [Column("work_day")]
    [StringLength(50)]
    public string WorkDay { get; set; } = null!;

    [Column("start_time")]
    public TimeOnly StartTime { get; set; }

    [Column("end_time")]
    public TimeOnly EndTime { get; set; }

    [ForeignKey("DoctorId")]
    [InverseProperty("DoctorSchedules")]
    public virtual Doctor? Doctor { get; set; }
}
