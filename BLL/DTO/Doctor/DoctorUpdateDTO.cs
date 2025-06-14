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
        // User properties (excluding password)
        //public int UserId { get; set; }
        //public string Username { get; set; } = null!;
        // public string Password { get; set; } = null!;
        //public string? Email { get; set; }
        
        [Phone(ErrorMessage = "Invalid phone number format")]
        [StringLength(20, ErrorMessage = "Phone number cannot exceed 20 characters")]
        public string? PhoneNumber { get; set; }
        
        [StringLength(100, ErrorMessage = "Full name cannot exceed 100 characters")]
        public string? FullName { get; set; }
        
        public DateOnly? DateOfBirth { get; set; }
        
        [StringLength(10, ErrorMessage = "Gender cannot exceed 10 characters")]
        public string? Gender { get; set; }
        
        public string? Address { get; set; }
        //public string UserRole { get; set; } = null!;
        public bool IsActive { get; set; }
        //public bool IsVerified { get; set; }

        // Doctor properties
        [Required(ErrorMessage = "Doctor ID is required")]
        public int DoctorId { get; set; }
        public string? DoctorImage { get; set; }
        public string? Bio { get; set; }
    }
}
