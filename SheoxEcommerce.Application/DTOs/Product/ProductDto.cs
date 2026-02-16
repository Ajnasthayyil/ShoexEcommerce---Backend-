using System.Collections.Generic;

namespace ShoexEcommerce.Application.DTOs.Product
{
    public class ProductDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public string? Description { get; set; }

        public int BrandId { get; set; }
        public string BrandName { get; set; } = null!;

        public int GenderId { get; set; }
        public string GenderName { get; set; } = null!;

        public bool IsActive { get; set; }

        public List<int> SizeIds { get; set; } = new();
        public List<ProductImageDto> Images { get; set; } = new();
    }
}
