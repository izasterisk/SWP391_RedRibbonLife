using System.ComponentModel.DataAnnotations;

namespace BLL.DTO.TestResult;

public class TestResultCreateDTO
{
    //Test Result
    // [Required(ErrorMessage = "Test result ID is required")]
    // public int TestResultId { get; set; }

    [Required(ErrorMessage = "Appointment ID is required")]
    [Range(1, int.MaxValue, ErrorMessage = "Appointment ID must be a positive number")]
    public int AppointmentId { get; set; }

    [Required(ErrorMessage = "Patient ID is required")]
    [Range(1, int.MaxValue, ErrorMessage = "Patient ID must be a positive number")]
    public int PatientId { get; set; }

    [Required(ErrorMessage = "Doctor ID is required")]
    [Range(1, int.MaxValue, ErrorMessage = "Doctor ID must be a positive number")]
    public int DoctorId { get; set; }

    // [Required(ErrorMessage = "Test type ID is required")]
    [Range(1, int.MaxValue, ErrorMessage = "Test type ID must be a positive number")]
    public int? TestTypeId { get; set; }

    [StringLength(255, ErrorMessage = "Result value cannot exceed 255 characters")]
    public string? ResultValue { get; set; }

    public string? Notes { get; set; }
}