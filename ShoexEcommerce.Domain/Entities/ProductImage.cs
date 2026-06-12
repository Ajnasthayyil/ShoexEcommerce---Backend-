using ShoexEcommerce.Domain.Common;

namespace ShoexEcommerce.Domain.Entities
{
    public class ProductImage : BaseEntity
    {
        public string Url { get; set; } = null!;
        public string PublicId { get; set; } = null!;
        public int ProductId { get; set; }
        public Product Product { get; set; } = null!;
    }

}
