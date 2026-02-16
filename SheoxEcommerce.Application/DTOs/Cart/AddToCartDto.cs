using System.ComponentModel.DataAnnotations;

namespace ShoexEcommerce.Application.DTOs.Cart
{
    public class AddToCartDto
    {
        [Required] public int ProductId { get; set; }
        [Required] public int SizeId { get; set; }
        [Range(1, 999)] public int Quantity { get; set; } = 1;
    }
}
