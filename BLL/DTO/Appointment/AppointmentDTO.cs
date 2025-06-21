using System.ComponentModel.DataAnnotations;
using BLL.Utils;

namespace BLL.DTO.Appointment;

public class AppointmentDTO
{
    public int AppointmentId { get; set; }
    
    [Required(ErrorMessage = "Patient ID is required")]
    [Range(1, int.MaxValue, ErrorMessage = "Patient ID must be a positive number")]
    public int PatientId { get; set; }

    [Required(ErrorMessage = "Doctor ID is required")]
    [Range(1, int.MaxValue, ErrorMessage = "Doctor ID must be a positive number")]
    public int DoctorId { get; set; }

    [Required(ErrorMessage = "Appointment date is required")]
    public DateOnly AppointmentDate { get; set; }

    [Required(ErrorMessage = "Appointment time is required")]
    [TimeValidator]
    public TimeOnly AppointmentTime { get; set; }

    [AllowedValues("Appointment", "Medication", ErrorMessage = "Appointment type must be either 'Appointment' or 'Medication'")]
    public string? AppointmentType { get; set; }

    [AllowedValues("Scheduled", "Confirmed", "Completed", "Cancelled", ErrorMessage = "Status must be one of: Scheduled, Confirmed, Completed, Cancelled")]
    public string? Status { get; set; }

    public bool? IsAnonymous { get; set; }
}