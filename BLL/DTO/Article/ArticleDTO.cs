using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.DTO.Article
{
    public class ArticleDTO
    {
        //Article
        // public int ArticleId { get; set; }
        [Required(ErrorMessage = "Title is required.")]
        [StringLength(200, ErrorMessage = "Title must not exceed 200 characters.")]
        public string Title { get; set; } = null!;
        [Required(ErrorMessage = "Content is required.")]
        public string Content { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "Category ID must be a positive number")]
        public int? CategoryId { get; set; }
        //public bool? IsActive { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "User ID must be a positive number")]
        public int? UserId { get; set; }
        public DateOnly CreatedDate { get; set; } = DateOnly.FromDateTime(DateTime.Now);
        //Category
        //public string CategoryName { get; set; } = null!;
        //public int? CategoryId { get; set; }
    }
}
