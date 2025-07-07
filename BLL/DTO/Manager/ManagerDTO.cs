namespace BLL.DTO.Manager;
using System.ComponentModel.DataAnnotations;

public class ManagerDTO
{
    // public int UserId { get; set; }
        
    [Required(ErrorMessage = "Username is required")]
    [StringLength(50, ErrorMessage = "Username cannot exceed 50 characters")]
    public string Username { get; set; }
        
    [Required(ErrorMessage = "Password is required")]
    [StringLength(255, MinimumLength = 6, ErrorMessage = "Password must be between 6 and 255 characters")]
    public string Password { get; set; }
        
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email format")]
    [StringLength(100, ErrorMessage = "Email cannot exceed 100 characters")]
    public string? Email { get; set; }
        
    [Phone(ErrorMessage = "Invalid phone number format")]
    [StringLength(20, MinimumLength = 7, ErrorMessage = "Phone number must be between 7-20 characters")]
    public string? PhoneNumber { get; set; }
        
    [Required(ErrorMessage = "Full name is required")]
    [StringLength(100, ErrorMessage = "Full name cannot exceed 100 characters")]
    public string? FullName { get; set; }
        
    public DateOnly? DateOfBirth { get; set; }
        
    [Required(ErrorMessage = "Gender is required")]
    [RegularExpression("^(Male|Female)$", ErrorMessage = "Gender must be either 'Male' or 'Female'")]
    public string? Gender { get; set; }
        
    [StringLength(500, ErrorMessage = "Address cannot exceed 500 characters")]
    public string? Address { get; set; }        
    // public bool IsVerified { get; set; } = false;
    //public string UserRole { get; set; }
    //public bool IsActive { get; set; } = true;
} 