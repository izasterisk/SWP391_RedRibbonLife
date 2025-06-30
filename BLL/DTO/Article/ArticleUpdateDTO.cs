using System.ComponentModel.DataAnnotations;

namespace BLL.DTO.Article;

public class ArticleUpdateDTO
{
    //Article
    public int ArticleId { get; set; }
    [StringLength(200, ErrorMessage = "Title must not exceed 200 characters.")]
    public string? Title { get; set; }
    public string? Content { get; set; }
    [Range(1, int.MaxValue, ErrorMessage = "Category ID must be a positive number")]
    public int? CategoryId { get; set; }
    public bool? IsActive { get; set; }
    [Range(1, int.MaxValue, ErrorMessage = "User ID must be a positive number")]
    public int? UserId { get; set; }
    // public DateOnly CreatedDate { get; set; }
    // public string? Author { get; set; }
        
    //Category
    //public string CategoryName { get; set; } = null!;
    //public int? CategoryId { get; set; }
}