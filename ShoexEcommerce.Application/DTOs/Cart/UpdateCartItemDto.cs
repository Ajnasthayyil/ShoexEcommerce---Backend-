using System.ComponentModel.DataAnnotations;

namespace ShoexEcommerce.Application.DTOs.Cart
{
    public class UpdateCartItemDto
    {
        [Required] public int CartItemId { get; set; }
        [Range(1, 999)] public int Quantity { get; set; }
    }
}
