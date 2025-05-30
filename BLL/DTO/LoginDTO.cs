using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.DTO
{
    public class LoginDTO
    {
        //[Required]
        //public string Policy { get; set; }
        [Required]
        public string Username { get; set; }
        [Required]
        public string Password { get; set; }
        //public string Email { get; set; }
        //[Required]
        //public string UserRole { get; set; }
    }
}
