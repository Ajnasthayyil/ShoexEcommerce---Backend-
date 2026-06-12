namespace ShoexEcommerce.Application.DTOs.Product
{
    public class ProductImageDto
    {
        public int Id { get; set; }
        public string Url { get; set; } = null!;
        public bool IsPrimary { get; set; }
    }
}
