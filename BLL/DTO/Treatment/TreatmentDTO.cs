using System.ComponentModel.DataAnnotations;
using BLL.Utils;

namespace BLL.DTO.Treatment;

public class TreatmentDTO
{
    //Treatment
    [Range(1, int.MaxValue, ErrorMessage = "Treatment ID must be a positive number")]
    public int TreatmentId { get; set; }

    // [Range(1, int.MaxValue, ErrorMessage = "Test Result ID must be a positive number")]
    // public int TestResultId { get; set; }

    // [Required(ErrorMessage = "Regimen ID is required")]
    // [Range(1, int.MaxValue, ErrorMessage = "Regimen ID must be a positive number")]
    // public int RegimenId { get; set; }

    [Required(ErrorMessage = "Start date is required")]
    // [FutureDate]
    public DateOnly StartDate { get; set; }

    [Required(ErrorMessage = "End date is required")]
    // [FutureDate]
    // [DateRange(nameof(StartDate))]
    public DateOnly EndDate { get; set; }

    [StringLength(50, ErrorMessage = "Status cannot exceed 50 characters")]
    [RegularExpression("^(Active|Stopped|Paused)$", ErrorMessage = "Status must be one of: Active, Stopped, Paused")]
    public string? Status { get; set; }

    public string? Notes { get; set; }
    
    //Doctor
    public int? DoctorId { get; set; }
    public string? DoctorName { get; set; }
    public string? DoctorImage { get; set; }
    
    //Patient
    public int? PatientId { get; set; }
    public string? PatientName { get; set; }
    public string? BloodType { get; set; }
    public bool? IsPregnant { get; set; }
    public string? SpecialNotes { get; set; }
    
    //ARVRegimen
    [Required(ErrorMessage = "Regimen ID is required")]
    [Range(1, int.MaxValue, ErrorMessage = "Regimen ID must be a positive number")]
    public int RegimenId { get; set; }
    public string? RegimenName { get; set; }
    public int? Component1Id { get; set; }
    public int? Component2Id { get; set; }
    public int? Component3Id { get; set; }
    public int? Component4Id { get; set; }
    public string? Description { get; set; }
    public string? SuitableFor { get; set; }
    public string? SideEffects { get; set; }
    public string? UsageInstructions { get; set; }
    public int? Frequency { get; set; }
    
    //Appointment
    public int? AppointmentId { get; set; }
    public DateOnly? AppointmentDate { get; set; }
    public TimeOnly? AppointmentTime { get; set; }
    public string? AppointmentType { get; set; }
    public string? AppointmentStatus { get; set; }
    public bool? IsAnonymous { get; set; }
    
    //Test Result
    [Range(1, int.MaxValue, ErrorMessage = "Test Result ID must be a positive number")]
    public int TestResultId { get; set; }
    
    [Required(ErrorMessage = "Test type ID is required")]
    [Range(1, int.MaxValue, ErrorMessage = "Test type ID must be a positive number")]
    public int TestTypeId { get; set; }
    
    [Required(ErrorMessage = "Test type name is required")]
    [StringLength(200, ErrorMessage = "Test type name cannot exceed 200 characters")]
    public string TestTypeName { get; set; } = null!;

    [StringLength(255, ErrorMessage = "Result value cannot exceed 255 characters")]
    public string? ResultValue { get; set; }

    public string? TestResultNotes { get; set; }
}