using System.ComponentModel.DataAnnotations;
using BLL.Utils;

namespace BLL.DTO.Appointment;

public class DoctorsAvailableDTO
{
    [Required(ErrorMessage = "Appointment date is required")]
    [FutureDate]
    public DateOnly AppointmentDate { get; set; }

    [Required(ErrorMessage = "Appointment time is required")]
    [TimeValidator]
    [WorkingHours]
    public TimeOnly AppointmentTime { get; set; }

    [Required(ErrorMessage = "Doctor ID is required")]
    [Range(1, int.MaxValue, ErrorMessage = "Doctor ID must be a positive number")]
    public int DoctorId { get; set; }
    
    public string? DoctorName { get; set; }
}