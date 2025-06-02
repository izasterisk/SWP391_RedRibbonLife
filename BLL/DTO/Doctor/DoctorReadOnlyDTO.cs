using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.DTO.Doctor
{
    public class DoctorReadOnlyDTO
    {
        // User properties (excluding password)
        public int UserId { get; set; }
        public string Username { get; set; } = null!;
        // public string Password { get; set; } = null!;
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? FullName { get; set; }
        public DateOnly? DateOfBirth { get; set; }
        public string? Gender { get; set; }
        public string? Address { get; set; }
        public string UserRole { get; set; } = null!;
        public bool IsActive { get; set; }

        // Doctor properties
        public int DoctorId { get; set; }
        public string? DoctorImage { get; set; }
        public string? Bio { get; set; }
    }
} 