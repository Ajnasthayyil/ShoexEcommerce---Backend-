using System.ComponentModel.DataAnnotations;

namespace ShoexEcommerce.Application.DTOs.Order
{
    public class CancelOrderDto
    {
        [Required]
        public int OrderId { get; set; }

        public string? Reason { get; set; } 
    }
}