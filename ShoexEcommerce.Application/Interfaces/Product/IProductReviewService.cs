using ShoexEcommerce.Application.Common;
using ShoexEcommerce.Application.DTOs.Product;

namespace ShoexEcommerce.Application.Interfaces.Product
{
    public interface IProductReviewService
    {
        Task<ApiResponse<string>> AddReviewAsync(int userId, AddProductReviewDto dto, CancellationToken ct = default);
        Task<ApiResponse<List<ProductReviewDto>>> GetReviewsByProductIdAsync(int productId, CancellationToken ct = default);
    }
}
