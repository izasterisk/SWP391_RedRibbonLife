using System.ComponentModel.DataAnnotations;

namespace BLL.DTO.Email;

public class ResponseDTO
{
    [Required(ErrorMessage = "Email là bắt buộc")]
    [EmailAddress(ErrorMessage = "Email không hợp lệ")]
    public string Email { get; set; }

    [Required(ErrorMessage = "Mã xác thực là bắt buộc")]
    [StringLength(6, MinimumLength = 6, ErrorMessage = "Mã xác thực phải có 6 ký tự")]
    public string VerifyCode { get; set; }
}