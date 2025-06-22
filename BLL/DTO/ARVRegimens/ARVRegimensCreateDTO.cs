using System.ComponentModel.DataAnnotations;

namespace BLL.DTO.ARVRegimens;

public class ARVRegimensCreateDTO
{
    // public int RegimenId { get; set; }

    [Required(ErrorMessage = "Regimen name is required")]
    [StringLength(100, ErrorMessage = "Regimen name cannot exceed 100 characters")]
    public string RegimenName { get; set; } = null!;

    [Required(ErrorMessage = "Regimen Code name is required")]
    [StringLength(20, ErrorMessage = "Regimen code cannot exceed 20 characters")]
    public string? RegimenCode { get; set; }

    [Required(ErrorMessage = "Components are required")]
    public string Components { get; set; } = null!;

    public string? Description { get; set; }

    public string? SuitableFor { get; set; }

    public string? SideEffects { get; set; }

    public string? UsageInstructions { get; set; }

    // public bool? IsActive { get; set; }
}