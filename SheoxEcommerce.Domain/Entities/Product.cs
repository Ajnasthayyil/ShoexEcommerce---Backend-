using ShoexEcommerce.Domain.Common;

namespace ShoexEcommerce.Domain.Entities
{
    public class Product : BaseEntity
    {
        public string Name { get; set; } = null!;
        public decimal Price { get; set; }
        //public int Stock { get; set; } 
        public string? Description { get; set; }

        public int BrandId { get; set; }
        public Brand Brand { get; set; } = null!;

        public int GenderId { get; set; }
        public Gender Gender { get; set; } = null!;

        public ICollection<ProductImage> Images { get; set; } = new List<ProductImage>();

        public ICollection<ProductSize> ProductSizes { get; set; } = new List<ProductSize>();
    }
}