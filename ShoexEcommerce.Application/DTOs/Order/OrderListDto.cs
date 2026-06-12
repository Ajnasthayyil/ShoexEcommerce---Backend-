namespace ShoexEcommerce.Application.DTOs.Order
{
    public class OrderListDto
    {
        public int OrderId { get; set; }
        public string CustomerName { get; set; } = null!;
        public string Status { get; set; } = null!;
        public decimal TotalAmount { get; set; }
        public DateTime CreatedOn { get; set; }
        public string? PaymentMethod { get; set; }

        // for admin table 
        public string? ProductName { get; set; }
        public decimal? Price { get; set; }
        public int? Quantity { get; set; }
        public decimal? Total { get; set; }

        public List<OrderItemDto> Items { get; set; } = new();

        public class OrderItemDto
        {
            public int ProductId { get; set; }
            public string ProductName { get; set; } = null!;
            public string ProductImageUrl { get; set; } = null!;
            public int SizeId { get; set; }
            public string SizeName { get; set; } = null!;
            public int Quantity { get; set; }
            public decimal UnitPrice { get; set; }
            public decimal TotalPrice { get; set; }
        }
    }
}
