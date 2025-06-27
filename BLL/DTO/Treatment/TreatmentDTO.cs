using System.ComponentModel.DataAnnotations;
using BLL.Utils;

namespace BLL.DTO.Treatment;

public class TreatmentDTO
{
    [Range(1, int.MaxValue, ErrorMessage = "Treatment ID must be a positive number")]
    public int TreatmentId { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Test Result ID must be a positive number")]
    public int? TestResultId { get; set; }

    [Required(ErrorMessage = "Regimen ID is required")]
    [Range(1, int.MaxValue, ErrorMessage = "Regimen ID must be a positive number")]
    public int RegimenId { get; set; }

    [Required(ErrorMessage = "Start date is required")]
    [FutureDate]
    public DateOnly StartDate { get; set; }

    [Required(ErrorMessage = "End date is required")]
    [FutureDate]
    [DateRange(nameof(StartDate))]
    public DateOnly EndDate { get; set; }

    [StringLength(50, ErrorMessage = "Status cannot exceed 50 characters")]
    [RegularExpression("^(Active|Stopped|Paused)$", ErrorMessage = "Status must be one of: Active, Stopped, Paused")]
    public string? Status { get; set; }

    public string? Notes { get; set; }
}