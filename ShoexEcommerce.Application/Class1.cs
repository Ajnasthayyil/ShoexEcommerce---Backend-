namespace ShoexEcommerce.Application.DTOs.Products
{
    public class ProductDetailsDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public decimal Price { get; set; }
        public string Description { get; set; } = null!;
        public string Brand { get; set; } = null!;
        public string Gender { get; set; } = null!;
        public List<string> Sizes { get; set; } = new();
        public List<string> Images { get; set; } = new();
    }
}
