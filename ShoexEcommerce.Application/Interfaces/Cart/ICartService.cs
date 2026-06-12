using ShoexEcommerce.Application.Common;
using ShoexEcommerce.Application.DTOs.Cart;
using System.Threading;

namespace ShoexEcommerce.Application.Interfaces.Cart
{
    public interface ICartService
    {
        Task<ApiResponse<CartDto>> GetMyCartAsync(int userId, CancellationToken ct = default);
        Task<ApiResponse<CartItemDto>> AddAsync(int userId, AddToCartDto dto, CancellationToken ct = default);
        Task<ApiResponse<CartItemDto>> UpdateAsync(int userId, UpdateCartItemDto dto, CancellationToken ct = default);
        Task<ApiResponse<string>> DeleteItemAsync(int userId, int cartItemId, CancellationToken ct = default);
        Task<ApiResponse<string>> ClearAsync(int userId, CancellationToken ct = default);
    }
}
