using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.DTO.Category
{
    public class CategoryUpdateDTO
    {
        [Required(ErrorMessage = "Category ID cannot be null")]
        public int CategoryId { get; set; }
        [Required(ErrorMessage = "Category name cannot be null")]
        public string CategoryName { get; set; } = null!;
        public bool? IsActive { get; set; }
    }
}