namespace ShoexEcommerce.Application.DTOs.Products
{
    public class ProductListDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }

        public string BrandName { get; set; } = string.Empty;
        public string GenderName { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
    }
}
