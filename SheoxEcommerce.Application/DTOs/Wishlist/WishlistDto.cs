namespace ShoexEcommerce.Application.DTOs.Wishlist
{
    public class WishlistDto
    {
        public List<WishlistProductDto> Items { get; set; } = new();
    }

    public class WishlistProductDto
    {
        public int ProductId { get; set; }
        public string Name { get; set; } = "";
        public decimal Price { get; set; }
        public string? ImageUrl { get; set; }
        public bool IsActive { get; set; }
    }
}
