namespace ShoexEcommerce.Application.DTOs.Order
{
    public class OrderListDto
    {
        public int OrderId { get; set; }
        public string CustomerName { get; set; } = null!;
        public string Status { get; set; } = null!;
        public decimal TotalAmount { get; set; }
        public DateTime CreatedOn { get; set; }

        // for admin table 
        public string? ProductName { get; set; }
        public decimal? Price { get; set; }
        public int? Quantity { get; set; }
        public decimal? Total { get; set; }
    }
}
