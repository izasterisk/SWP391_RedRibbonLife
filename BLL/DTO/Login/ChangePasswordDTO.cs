using System.ComponentModel.DataAnnotations;

namespace BLL.DTO.Login;

public class ChangePasswordDTO
{
    //[Required]
    //public string Policy { get; set; }
    // [Required]
    // public string Username { get; set; }
    [Required]
    public string Password { get; set; }
    [Required]
    public string newPassword { get; set; }
    [Required]
    public string Email { get; set; }
    //[Required]
    //public string UserRole { get; set; }
}