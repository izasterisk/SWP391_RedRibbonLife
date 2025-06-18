using System.ComponentModel.DataAnnotations;

namespace BLL.DTO.Patient;

public class PatientUpdateDTO
{
    // Các thuộc tính từ Patient
    [Required(ErrorMessage = "Patient ID is required")]
    public int PatientId { get; set; }
    // public int UserId { get; set; }
    
    [StringLength(10, ErrorMessage = "Blood type cannot exceed 10 characters")]
    public string? BloodType { get; set; }
    
    public bool? IsPregnant { get; set; }
    
    [StringLength(500, ErrorMessage = "Special notes cannot exceed 500 characters")]
    public string? SpecialNotes { get; set; }


    // Các thuộc tính từ User
    //public int UserId { get; set; };
    // public string Username { get; set; } = null!;
    // public string Password { get; set; } = null!;
    [EmailAddress(ErrorMessage = "Invalid email address.")]
    public string? Email { get; set; }
    
    [RegularExpression(@"^\d+$", ErrorMessage = "Phone number must contain only digits")]
    [StringLength(20, ErrorMessage = "Phone number cannot exceed 20 characters")]
    public string? PhoneNumber { get; set; }
    
    [StringLength(100, ErrorMessage = "Full name cannot exceed 100 characters")]
    public string? FullName { get; set; }
    
    public DateOnly? DateOfBirth { get; set; }
    
    [StringLength(10, ErrorMessage = "Gender cannot exceed 10 characters")]
    [RegularExpression("^(Male|Female)$", ErrorMessage = "Gender must be either 'Male' or 'Female'")]
    public string? Gender { get; set; }
    
    [StringLength(500, ErrorMessage = "Address cannot exceed 500 characters")]
    public string? Address { get; set; }
    // public string UserRole { get; set; } = null!;
    public bool IsActive { get; set; }
    // public bool IsVerified { get; set; }
}