namespace ShoexEcommerce.Application.DTOs.Order
{
    public class PlaceOrderResponseDto
    {
        public int OrderId { get; set; }
        public string? RazorpayOrderId { get; set; }
        public decimal Amount { get; set; }
        public string? Key { get; set; }
    }
}
