using System.ComponentModel.DataAnnotations;

namespace BLL.DTO.Email;

public class TreatmentEmailRequestDTO
{
    [Required(ErrorMessage = "TreatmentId is required")]
    [Range(1, int.MaxValue, ErrorMessage = "TreatmentId must be a positive number")]
    public int TreatmentId { get; set; }
}