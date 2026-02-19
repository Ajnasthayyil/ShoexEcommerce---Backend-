namespace ShoexEcommerce.Application.DTOs.Order
{
    public class PlaceOrderDto
    {
        public int? AddressId { get; set; }

        public string PaymentMethod { get; set; } = "COD";
    }
}