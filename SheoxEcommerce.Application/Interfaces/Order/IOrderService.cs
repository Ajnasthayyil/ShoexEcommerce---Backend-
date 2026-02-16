using ShoexEcommerce.Application.Common;
using ShoexEcommerce.Application.DTOs.Order;
using ShoexEcommerce.Domain.Enums;

namespace ShoexEcommerce.Application.Interfaces.Order
{
    public interface IOrderService
    {
        Task<ApiResponse<string>> PlaceOrderAsync(int userId, PlaceOrderDto dto, CancellationToken ct = default);
        Task<ApiResponse<string>> BuyNowAsync(int userId, BuyNowDto dto, CancellationToken ct = default);

        Task<ApiResponse<List<OrderListDto>>> GetMyOrdersAsync(int userId, CancellationToken ct = default);
        Task<ApiResponse<OrderDetailDto>> GetMyOrderDetailAsync(int userId, int orderId, CancellationToken ct = default);

        // Admin
        Task<ApiResponse<List<OrderListDto>>> AdminGetOrdersAsync(CancellationToken ct = default);
        Task<ApiResponse<OrderDetailDto>> AdminGetOrderDetailAsync(int orderId, CancellationToken ct = default);
        Task<ApiResponse<string>> AdminUpdateStatusAsync(
             int orderId,
             OrderStatus status,
             CancellationToken ct = default);
            }
}
