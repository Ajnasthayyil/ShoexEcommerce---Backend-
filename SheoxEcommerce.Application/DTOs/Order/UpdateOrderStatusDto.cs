using ShoexEcommerce.Domain.Enums;

namespace ShoexEcommerce.Application.DTOs.Order
{
    public class UpdateOrderStatusDto
    {
        public OrderStatus Status { get; set; }
    }
}
