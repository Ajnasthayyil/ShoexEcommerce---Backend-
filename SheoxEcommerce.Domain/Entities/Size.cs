using ShoexEcommerce.Domain.Common;

namespace ShoexEcommerce.Domain.Entities
{
    public class Size : BaseEntity
    {
        public string Name { get; set; } = null!;

        public ICollection<ProductSize> ProductSizes { get; set; }
    }
}