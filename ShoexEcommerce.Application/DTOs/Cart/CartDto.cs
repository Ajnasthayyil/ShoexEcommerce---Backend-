namespace ShoexEcommerce.Application.DTOs.Cart
{
    public class CartDto
    {
        public int CartId { get; set; }
        public List<CartItemDto> Items { get; set; } = new();
        public decimal Total { get; set; }
    }

    public class CartItemDto
    {
        public int CartItemId { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; } = "";
        public decimal Price { get; set; }
        public int SizeId { get; set; }
        public string SizeName { get; set; } = "";
        public int Quantity { get; set; }
        public decimal LineTotal { get; set; }
        public string? ImageUrl { get; set; }
    }
}
