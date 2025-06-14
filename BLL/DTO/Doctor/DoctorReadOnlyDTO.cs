using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.DTO.Doctor
{
    public class DoctorReadOnlyDTO
    {
        // User properties (excluding password)
        // public int UserId { get; set; }
        // public string Username { get; set; } = null!;
        // public string Password { get; set; } = null!;
        
        [EmailAddress(ErrorMessage = "Invalid email format")]
        [StringLength(100, ErrorMessage = "Email cannot exceed 100 characters")]
        public string? Email { get; set; }
        
        [Phone(ErrorMessage = "Invalid phone number format")]
        [StringLength(20, ErrorMessage = "Phone number cannot exceed 20 characters")]
        public string? PhoneNumber { get; set; }
        
        [StringLength(100, ErrorMessage = "Full name cannot exceed 100 characters")]
        public string? FullName { get; set; }
        
        public DateOnly? DateOfBirth { get; set; }
        
        [StringLength(10, ErrorMessage = "Gender cannot exceed 10 characters")]
        public string? Gender { get; set; }
        
        public string? Address { get; set; }
        
        [Required(ErrorMessage = "User role is required")]
        [StringLength(50, ErrorMessage = "User role cannot exceed 50 characters")]
        public string UserRole { get; set; } = null!;
        
        public bool IsActive { get; set; }
        public bool IsVerified { get; set; }

        // Doctor properties
        public int DoctorId { get; set; }
        public string? DoctorImage { get; set; }
        public string? Bio { get; set; }
    }
} 