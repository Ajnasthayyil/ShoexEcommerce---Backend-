using System.ComponentModel.DataAnnotations;

namespace ShoexEcommerce.Application.DTOs.Product
{
    public class UpdateStockDto
    {
        [Required]
        public int Stock { get; set; }
    }
}
