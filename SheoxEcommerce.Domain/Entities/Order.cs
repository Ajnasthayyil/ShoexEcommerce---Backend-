using ShoexEcommerce.Domain.Common;
using ShoexEcommerce.Domain.Enums;

namespace ShoexEcommerce.Domain.Entities
{
    public class Order : BaseEntity
    {
        public int UserId { get; set; }
        public User? User { get; set; }

        public string PaymentMethod { get; set; } = "COD";
        public OrderStatus Status { get; set; } = OrderStatus.Ordered;

        public decimal SubTotal { get; set; }
        public decimal TotalAmount { get; set; }

        public int? ShippingAddressId { get; set; }
        public Address? ShippingAddress { get; set; }


        public List<OrderItem> Items { get; set; } = new();
    }
}
