using SheoxEcommerce.Application.DTOs.Size;
using ShoexEcommerce.Application.Common;
namespace ShoexEcommerce.Application.Interfaces.Product

{
    public interface ISizeService
    {
        Task<ApiResponse<SizeDto>> CreateAsync(CreateSizeDto dto);
        Task<ApiResponse<List<SizeDto>>> GetAllAsync();
        Task<ApiResponse<List<SizeDto>>> GetActiveAsync();
        Task<ApiResponse<SizeDto>> ToggleAsync(int id);
    }
}
