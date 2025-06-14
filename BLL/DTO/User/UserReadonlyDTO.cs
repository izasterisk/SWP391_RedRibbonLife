using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.DTO.User
{
    public class UserReadonlyDTO
    {
        public int UserId { get; set; }
        public string Username { get; set; }
        //public string Password { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? FullName { get; set; }
        public DateOnly? DateOfBirth { get; set; }
        public string? Gender { get; set; }
        public string? Address { get; set; }
        public string UserRole { get; set; }
        public bool IsActive { get; set; } = true;
        public bool IsVerified { get; set; }
    }
}
