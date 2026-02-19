namespace ShoexEcommerce.Application.DTOs.Order
{
    public class OrderListDto
    {
        public int OrderId { get; set; }
        public string CustomerName { get; set; } = "";
        public string Status { get; set; } = "";
        public decimal TotalAmount { get; set; }
        public DateTime CreatedOn { get; set; }

        public List<OrderListItemDto> Items { get; set; } = new();
    }

    public class OrderListItemDto
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = "";
        public int SizeId { get; set; }
        public string SizeName { get; set; } = "";
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice { get; set; }
    }
}