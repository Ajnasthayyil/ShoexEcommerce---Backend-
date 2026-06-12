using Microsoft.AspNetCore.Http;
using ShoexEcommerce.Application.Common;
using ShoexEcommerce.Application.DTOs.Product;
using ShoexEcommerce.Application.DTOs.Products;

namespace ShoexEcommerce.Application.Interfaces.Product
{
    public interface IProductService
    {
        Task<ApiResponse<List<ProductListDto>>> GetActiveProductsAsync(CancellationToken ct = default);
        Task<ApiResponse<ProductDto>> GetActiveProductByIdAsync(int id, CancellationToken ct = default);

        Task<ApiResponse<ProductDto>> CreateAsync(CreateProductDto dto, CancellationToken ct = default);
        Task<ApiResponse<ProductDto>> GetByIdAsync(int id, CancellationToken ct = default);
        Task<ApiResponse<List<ProductDto>>> GetAllAsync(CancellationToken ct = default);

        Task<ApiResponse<ProductDto>> UpdateAsync(int id, UpdateProductDto dto, CancellationToken ct = default);
        Task<ApiResponse<string>> ToggleAsync(int id, CancellationToken ct = default);
        Task<ApiResponse<string>> DeleteAsync(int id, CancellationToken ct = default);
        Task<ApiResponse<string>> UpdateSizeStockAsync(int productId, int sizeId,UpdateStockDto dto,CancellationToken ct = default);
        Task<ApiResponse<string>> AdjustSizeStockAsync(int productId,int sizeId, AdjustStockDto dto,CancellationToken ct = default);
        Task<ApiResponse<ProductDto>> AddImagesAsync(int productId, List<IFormFile> images, CancellationToken ct = default);
        Task<ApiResponse<string>> DeleteImageAsync(int productId, int imageId, CancellationToken ct = default);
    }
}
