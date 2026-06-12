using System.ComponentModel.DataAnnotations;

namespace ShoexEcommerce.Application.DTOs.Product
{
    public class AdjustStockDto
    {
        [Required]
        public int Delta { get; set; }  
    }
}