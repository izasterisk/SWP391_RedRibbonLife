using System.ComponentModel.DataAnnotations;

namespace BLL.DTO.Article;

public class ArticleReadOnlyDTO
{
    //Article
    public int ArticleId { get; set; }
    [Required(ErrorMessage = "Title is required.")]
    [StringLength(200, ErrorMessage = "Title must not exceed 200 characters.")]
    public string Title { get; set; } = null!;
    public string? Content { get; set; }
    public string? ThumbnailImage { get; set; }
    public int? CategoryId { get; set; }
    public bool? IsActive { get; set; }
        
    //Category
    public string CategoryName { get; set; } = null!;
    //public int? CategoryId { get; set; }
}