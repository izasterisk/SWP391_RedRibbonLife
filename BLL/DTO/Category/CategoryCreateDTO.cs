using System.ComponentModel.DataAnnotations;

namespace BLL.DTO.Category;

public class CategoryCreateDTO
{
    // public int CategoryId { get; set; }
    [Required(ErrorMessage = "Category name cannot be null")]
    public string CategoryName { get; set; } = null!;
    // public bool? IsActive { get; set; }
}