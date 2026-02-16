using ShoexEcommerce.Application.Common;
using ShoexEcommerce.Application.DTOs.Brand;


namespace ShoexEcommerce.Application.Interfaces.Brand
{
    public interface IBrandService
    {
        Task<ApiResponse<List<BrandDto>>> GetAllAsync();
        Task<ApiResponse<BrandDto>> GetByIdAsync(int id);
        Task<ApiResponse<BrandDto>> CreateAsync(CreateBrandDto dto);
        Task<ApiResponse<BrandDto>> UpdateAsync(int id, BrandUpdateDto dto);
        Task<ApiResponse<string>> ToggleActiveAsync(int id);
        Task<ApiResponse<string>> DeleteAsync(int id);
    }
}
