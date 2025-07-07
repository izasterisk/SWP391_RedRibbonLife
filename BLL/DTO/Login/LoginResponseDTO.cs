using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.DTO.Login
{
    public class LoginResponseDTO
    {
        public string Username { get; set; }
        public string? FullName { get; set; }
        public string token { get; set; }
    }
}
