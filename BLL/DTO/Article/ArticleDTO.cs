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
        public int ArticleId { get; set; }
        public string Title { get; set; } = null!;
        public string? Content { get; set; }
        public string? ThumbnailImage { get; set; }
        public string CategoryName { get; set; } = null!;
        public bool? IsActive { get; set; }
    }
}
