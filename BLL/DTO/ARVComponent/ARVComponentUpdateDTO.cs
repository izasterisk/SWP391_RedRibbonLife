using System.ComponentModel.DataAnnotations;

namespace BLL.DTO.ARVComponent;

public class ARVComponentUpdateDTO
{
    public int ComponentId { get; set; }
    
    [Required(ErrorMessage = "ARV component name is required")]
    [StringLength(10, ErrorMessage = "ARV component name cannot exceed 10 characters")]
    [RegularExpression(@"^[A-Z0-9]+(\s+[A-Z0-9]+)*$", ErrorMessage = "ARV component name can only contain uppercase letters, numbers and spaces")]
    public string? ComponentName { get; set; } = null!;

    [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters")]
    public string? Description { get; set; }
}