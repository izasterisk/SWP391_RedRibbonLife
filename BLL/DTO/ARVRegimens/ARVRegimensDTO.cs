using System.ComponentModel.DataAnnotations;

namespace BLL.DTO.ARVRegimens;

public class ARVRegimensDTO
{
    public int RegimenId { get; set; }

    [StringLength(100, ErrorMessage = "Regimen name cannot exceed 100 characters")]
    public string? RegimenName { get; set; } = null!;

    [Required(ErrorMessage = "Component 1 is required")]
    [Range(1, int.MaxValue, ErrorMessage = "Component 1 ID must be a positive number")]
    public int Component1Id { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Component 2 ID must be a positive number")]
    public int? Component2Id { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Component 3 ID must be a positive number")]
    public int? Component3Id { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Component 4 ID must be a positive number")]
    public int? Component4Id { get; set; }

    [StringLength(2000, ErrorMessage = "Description cannot exceed 2000 characters")]
    public string? Description { get; set; }

    [StringLength(1000, ErrorMessage = "Suitable for cannot exceed 1000 characters")]
    public string? SuitableFor { get; set; }

    [StringLength(2000, ErrorMessage = "Side effects cannot exceed 2000 characters")]
    public string? SideEffects { get; set; }

    [StringLength(2000, ErrorMessage = "Usage instructions cannot exceed 2000 characters")]
    public string? UsageInstructions { get; set; }
    
    [Required(ErrorMessage = "Frequency is required")]
    [Range(1, 2, ErrorMessage = "Frequency must be either 1 or 2")]
    public int Frequency { get; set; }

    public bool IsActive { get; set; }

    public bool IsCustomized { get; set; }
}