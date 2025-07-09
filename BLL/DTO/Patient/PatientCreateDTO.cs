using System.ComponentModel.DataAnnotations;

namespace BLL.DTO.Patient;

public class PatientCreateDTO
{
    //User
    // public int UserId { get; set; }

    [Required(ErrorMessage = "Username is required")]
    [StringLength(50, ErrorMessage = "Username cannot exceed 50 characters")]
    public string Username { get; set; } = null!;

    [Required(ErrorMessage = "Password is required")]
    [StringLength(255, ErrorMessage = "Password cannot exceed 255 characters")]
    public string Password { get; set; } = null!;

    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Email format is invalid")]
    [StringLength(100, ErrorMessage = "Email cannot exceed 100 characters")]
    public string Email { get; set; }

    [Phone(ErrorMessage = "Invalid phone number format")]
    [StringLength(20, MinimumLength = 7, ErrorMessage = "Phone number must be between 7-20 characters")]
    public string? PhoneNumber { get; set; }

    [Required(ErrorMessage = "FullName is required")]
    [StringLength(100, ErrorMessage = "Full name cannot exceed 100 characters")]
    public string? FullName { get; set; }

    public DateOnly? DateOfBirth { get; set; }

    [Required(ErrorMessage = "Gender is required")]
    [StringLength(10, ErrorMessage = "Gender cannot exceed 10 characters")]
    [RegularExpression("^(Male|Female)$", ErrorMessage = "Gender must be either Male or Female")]
    public string? Gender { get; set; }

    public string? Address { get; set; }

    // [Required(ErrorMessage = "User role is required")]
    // [StringLength(50, ErrorMessage = "User role cannot exceed 50 characters")]
    // [RegularExpression("^(Patient|Staff|Doctor|Manager|Admin)$", ErrorMessage = "User role must be one of: Patient, Staff, Doctor, Manager, Admin")]
    // public string UserRole { get; set; } = null!;

    // public bool IsActive { get; set; }
    
    // public bool IsVerified { get; set; }
    
    //Patient
    // public int PatientId { get; set; }

    [StringLength(5, ErrorMessage = "Blood type cannot exceed 5 characters")]
    [RegularExpression("^(A\\+|A-|B\\+|B-|AB\\+|AB-|O\\+|O-)$", ErrorMessage = "Blood type must be one of: A+, A-, B+, B-, AB+, AB-, O+, O-")]
    public string? BloodType { get; set; }

    public bool? IsPregnant { get; set; }

    [StringLength(500, ErrorMessage = "Special notes cannot exceed 500 characters")]
    public string? SpecialNotes { get; set; }
}