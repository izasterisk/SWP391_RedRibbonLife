using System.ComponentModel.DataAnnotations;

namespace BLL.DTO.Patient;

public class PatientOnlyDTO
{
    // Các thuộc tính từ Patient
    public int PatientId { get; set; }
    // public int? UserId { get; set; }

    [StringLength(10, ErrorMessage = "Blood type cannot exceed 10 characters")]
    public string? BloodType { get; set; }

    public bool? IsPregnant { get; set; }

    [StringLength(500, ErrorMessage = "Special notes cannot exceed 500 characters")]
    public string? SpecialNotes { get; set; }
}