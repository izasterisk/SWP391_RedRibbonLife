using System.ComponentModel.DataAnnotations;

namespace BLL.DTO.Article;

public class ArticleUpdateDTO
{
    //Article
    public int ArticleId { get; set; }
    [StringLength(200, ErrorMessage = "Title must not exceed 200 characters.")]
    public string? Title { get; set; }
    public string? Content { get; set; }
    public int? CategoryId { get; set; }
    public bool? IsActive { get; set; }
    public int? UserId { get; set; }
    // public DateOnly CreatedDate { get; set; }
    // public string? Author { get; set; }
        
    //Category
    //public string CategoryName { get; set; } = null!;
    //public int? CategoryId { get; set; }
}