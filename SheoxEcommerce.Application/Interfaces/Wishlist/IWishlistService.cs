using ShoexEcommerce.Application.Common;
using ShoexEcommerce.Application.DTOs.Wishlist;

namespace ShoexEcommerce.Application.Interfaces.Wishlist
{
    public interface IWishlistService
    {
        Task<ApiResponse<WishlistDto>> GetMyWishlistAsync(int userId, CancellationToken ct = default);
        Task<ApiResponse<string>> ToggleAsync(int userId, int productId, CancellationToken ct = default);
        Task<ApiResponse<string>> ClearAsync(int userId, CancellationToken ct = default);
    }
}
