using System.ComponentModel.DataAnnotations;

namespace BLL.DTO.DoctorSchedule;

public class DoctorScheduleDTO
{
    public int ScheduleId { get; set; }

    [Required(ErrorMessage = "Doctor ID is required")]
    [Range(1, int.MaxValue, ErrorMessage = "Doctor ID must be a positive number")]
    public int? DoctorId { get; set; }
    
    [Required(ErrorMessage = "Work day is required")]
    [StringLength(50, ErrorMessage = "Work day cannot exceed 50 characters")]
    [RegularExpression("^(Monday|Tuesday|Wednesday|Thursday|Friday|Saturday|Sunday)$", 
        ErrorMessage = "Work day must be one of: Monday, Tuesday, Wednesday, Thursday, Friday, Saturday, Sunday")]
    public string WorkDay { get; set; } = null!;

    [Required(ErrorMessage = "Start time is required")]
    public TimeOnly StartTime { get; set; }

    [Required(ErrorMessage = "End time is required")]
    public TimeOnly EndTime { get; set; }
}