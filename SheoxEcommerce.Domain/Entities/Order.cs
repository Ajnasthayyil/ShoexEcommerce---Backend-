using ShoexEcommerce.Domain.Common;
using ShoexEcommerce.Domain.Enums;

namespace ShoexEcommerce.Domain.Entities
{
    public class Order : BaseEntity
    {
        public int UserId { get; set; }
        public User User { get; set; } = null!;

        public OrderStatus Status { get; set; } = OrderStatus.Ordered;
        public string PaymentMethod { get; set; } = "COD";

        public decimal SubTotal { get; set; }
        public decimal TotalAmount { get; set; }

        public List<OrderItem> Items { get; set; } = new();
    }
}
