using System.ComponentModel.DataAnnotations;

namespace BLL.DTO.Email;

public class RequestDTO
{
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Email format is invalid")]
    public string Email { get; set; }
}