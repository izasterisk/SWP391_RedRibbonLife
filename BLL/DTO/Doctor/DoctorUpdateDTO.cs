using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.DTO.Doctor
{
    public class DoctorUpdateDTO
    {
        //User
        // public int UserId { get; set; }

        // [Required(ErrorMessage = "Username is required")]
        // [StringLength(50, ErrorMessage = "Username cannot exceed 50 characters")]
        // public string Username { get; set; } = null!;

        // [Required(ErrorMessage = "Password is required")]
        // [StringLength(255, ErrorMessage = "Password cannot exceed 255 characters")]
        // public string Password { get; set; } = null!;

        // [Required(ErrorMessage = "Email is required")]
        // [EmailAddress(ErrorMessage = "Email format is invalid")]
        // [StringLength(100, ErrorMessage = "Email cannot exceed 100 characters")]
        // public string? Email { get; set; }

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

        public bool? IsActive { get; set; }

        // public bool IsVerified { get; set; }
        
        //Doctor
        public int DoctorId { get; set; }

        //public int? UserId { get; set; }

        public string? DoctorImage { get; set; }

        public string? Bio { get; set; }
    }
}
