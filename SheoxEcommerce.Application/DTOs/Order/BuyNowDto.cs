using System.ComponentModel.DataAnnotations;

namespace ShoexEcommerce.Application.DTOs.Order
{
    public class BuyNowDto
    {
        [Required] public int ProductId { get; set; }
        [Required] public int SizeId { get; set; }
        [Required, Range(1, 20)] public int Quantity { get; set; }

        [Required] public int AddressId { get; set; }
        public string PaymentMethod { get; set; } = "COD";
    }
}