using System.ComponentModel.DataAnnotations;
namespace BLL.DTO.TestType;

public class TestTypeUpdateDTO
{
    [Required(ErrorMessage = "ID is required")]
    public int TestTypeId { get; set; }

    [Required(ErrorMessage = "Test type name is required")]
    [StringLength(200, ErrorMessage = "Test type name cannot exceed 200 characters")]
    public string TestTypeName { get; set; } = null!;

    [Required(ErrorMessage = "Unit is required")]
    [StringLength(50, ErrorMessage = "Unit cannot exceed 50 characters")]
    [RegularExpression(@"^(cells/mm³|copies/mL|mg/dL|g/L|IU/L|IU/mL|%|mmHg|S/C|N/A)$", 
        ErrorMessage = "Unit must be one of the following values: cells/mm³, copies/mL, mg/dL, g/L, IU/L, IU/mL, %, mmHg, S/C, N/A")]
    public string Unit { get; set; } = null!;

    [StringLength(1000, ErrorMessage = "Normal range cannot exceed 1000 characters")]
    public string? NormalRange { get; set; }
    
    public bool? IsActive { get; set; }
}