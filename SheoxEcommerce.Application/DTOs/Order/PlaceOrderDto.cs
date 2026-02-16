using System.ComponentModel.DataAnnotations;

namespace ShoexEcommerce.Application.DTOs.Order
{
    public class PlaceOrderDto
    {
        [Required]
        public int AddressId { get; set; }
        public string PaymentMethod { get; set; } = "COD";


    }
}
