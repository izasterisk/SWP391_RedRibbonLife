using System.ComponentModel.DataAnnotations;
using BLL.Utils;

namespace BLL.DTO.TestResult;

public class TestResultDTO
{
//Test Result
    [Required(ErrorMessage = "Test result ID is required")]
    public int TestResultId { get; set; }

    [Required(ErrorMessage = "Appointment ID is required")]
    [Range(1, int.MaxValue, ErrorMessage = "Appointment ID must be a positive number")]
    public int AppointmentId { get; set; }

    [Required(ErrorMessage = "Patient ID is required")]
    [Range(1, int.MaxValue, ErrorMessage = "Patient ID must be a positive number")]
    public int PatientId { get; set; }

    [Required(ErrorMessage = "Doctor ID is required")]
    [Range(1, int.MaxValue, ErrorMessage = "Doctor ID must be a positive number")]
    public int DoctorId { get; set; }

    [Required(ErrorMessage = "Test type ID is required")]
    [Range(1, int.MaxValue, ErrorMessage = "Test type ID must be a positive number")]
    public int TestTypeId { get; set; }

    [StringLength(255, ErrorMessage = "Result value cannot exceed 255 characters")]
    public string? ResultValue { get; set; }

    public string? Notes { get; set; }
    
    //Test Type
    // [Required(ErrorMessage = "ID is required")]
    // public int TestTypeId { get; set; }

    [Required(ErrorMessage = "Test type name is required")]
    [StringLength(200, ErrorMessage = "Test type name cannot exceed 200 characters")]
    public string TestTypeName { get; set; } = null!;

    [Required(ErrorMessage = "Unit is required")]
    [StringLength(50, ErrorMessage = "Unit cannot exceed 50 characters")]
    [RegularExpression(@"^(cells/mm³|copies/mL|mg/dL|g/L|IU/L|IU/mL|%|mmHg|S/C|N/A)$", 
        ErrorMessage = "Unit must be one of the following values: cells/mm³, copies/mL, mg/dL, g/L, IU/L, IU/mL, %, mmHg, S/C, N/A")]
    public string Unit { get; set; } = null!;

    [StringLength(1000, ErrorMessage = "Normal range cannot exceed 1000 characters")]
    public string? NormalRange { get; set; }
    
//User
    // public int UserId { get; set; }

    // [Required(ErrorMessage = "Username is required")]
    // [StringLength(50, ErrorMessage = "Username cannot exceed 50 characters")]
    // public string Username { get; set; } = null!;
    //
    // [Required(ErrorMessage = "Password is required")]
    // [StringLength(255, ErrorMessage = "Password cannot exceed 255 characters")]
    // public string Password { get; set; } = null!;

    // [Required(ErrorMessage = "Email is required")]
    // [EmailAddress(ErrorMessage = "Email format is invalid")]
    // [StringLength(100, ErrorMessage = "Email cannot exceed 100 characters")]
    // public string? Email { get; set; }

    // [Phone(ErrorMessage = "Phone number format is invalid")]
    // [StringLength(20, ErrorMessage = "Phone number cannot exceed 20 characters")]
    // public string? PhoneNumber { get; set; }

    // [Required(ErrorMessage = "FullName is required")]
    // [StringLength(100, ErrorMessage = "Full name cannot exceed 100 characters")]
    // public string? FullName { get; set; }
    //
    // public DateOnly? DateOfBirth { get; set; }
    //
    // [Required(ErrorMessage = "Gender is required")]
    // [StringLength(10, ErrorMessage = "Gender cannot exceed 10 characters")]
    // [RegularExpression("^(Male|Female)$", ErrorMessage = "Gender must be either Male or Female")]
    // public string? Gender { get; set; }

    // public string? Address { get; set; }

    // [Required(ErrorMessage = "User role is required")]
    // [StringLength(50, ErrorMessage = "User role cannot exceed 50 characters")]
    // [RegularExpression("^(Patient|Staff|Doctor|Manager|Admin)$", ErrorMessage = "User role must be one of: Patient, Staff, Doctor, Manager, Admin")]
    // public string UserRole { get; set; } = null!;

    // public bool IsActive { get; set; }
    //
    // public bool IsVerified { get; set; }
    
//Patient
    // public int PatientId { get; set; }
    
    public string PatientName { get; set; }

    [StringLength(5, ErrorMessage = "Blood type cannot exceed 5 characters")]
    [RegularExpression("^(A\\+|A-|B\\+|B-|AB\\+|AB-|O\\+|O-)$", ErrorMessage = "Blood type must be one of: A+, A-, B+, B-, AB+, AB-, O+, O-")]
    public string? BloodType { get; set; }

    public bool? IsPregnant { get; set; }

    [StringLength(500, ErrorMessage = "Special notes cannot exceed 500 characters")]
    public string? SpecialNotes { get; set; }
    
//Doctor
    // public int DoctorId { get; set; }

    public string DoctorName { get; set; }
    
    //public int? UserId { get; set; }

    // public string? DoctorImage { get; set; }

    // public string? Bio { get; set; }
    
//Appointment
    // public int AppointmentId { get; set; }
    
    // [Required(ErrorMessage = "Patient ID is required")]
    // [Range(1, int.MaxValue, ErrorMessage = "Patient ID must be a positive number")]
    // public int PatientId { get; set; }
    
    // public string? PatientName { get; set; }

    // [Required(ErrorMessage = "Doctor ID is required")]
    // [Range(1, int.MaxValue, ErrorMessage = "Doctor ID must be a positive number")]
    // public int DoctorId { get; set; }
    
    // public string? DoctorName { get; set; }

    [Required(ErrorMessage = "Appointment date is required")]
    [FutureDate]
    public DateOnly AppointmentDate { get; set; }

    [Required(ErrorMessage = "Appointment time is required")]
    [TimeValidator]
    [WorkingHours]
    public TimeOnly AppointmentTime { get; set; }

    [AllowedValues("Appointment", "Medication", ErrorMessage = "Appointment type must be either 'Appointment' or 'Medication'")]
    public string? AppointmentType { get; set; }

    [AllowedValues("Scheduled", "Confirmed", "Completed", "Cancelled", ErrorMessage = "Status must be one of: Scheduled, Confirmed, Completed, Cancelled")]
    public string? Status { get; set; }

    public bool? IsAnonymous { get; set; }
}