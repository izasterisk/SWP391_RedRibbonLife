using System.ComponentModel.DataAnnotations;

namespace BLL.DTO.Email;

public class RequestDTO
{
    [Required(ErrorMessage = "Email là bắt buộc")]
    [EmailAddress(ErrorMessage = "Email không hợp lệ")]
    public string Email { get; set; }
}