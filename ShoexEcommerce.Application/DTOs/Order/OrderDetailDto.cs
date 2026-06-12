namespace ShoexEcommerce.Application.DTOs.Order
{
    public class OrderDetailDto
    {
        public int OrderId { get; set; }
        public string Status { get; set; } = null!;
        public decimal SubTotal { get; set; }
        public decimal TotalAmount { get; set; }
        public string PaymentMethod { get; set; } = null!;
        public DateTime CreatedOn { get; set; }

        public string CustomerName { get; set; } = null!;
        public string CustomerEmail { get; set; } = null!;
        public string CustomerMobile { get; set; } = null!;

        public List<OrderItemDto> Items { get; set; } = new();

        public class OrderItemDto
        {
            public int ProductId { get; set; }
            public string ProductName { get; set; } = null!;
            public int SizeId { get; set; }
            public string SizeName { get; set; } = null!;
            public int Quantity { get; set; }
            public decimal UnitPrice { get; set; }
            public decimal Total { get; set; }
        }
    }
}
