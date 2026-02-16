using ShoexEcommerce.Application.Common;
using ShoexEcommerce.Application.DTOs.Address;

namespace ShoexEcommerce.Application.Interfaces.Address
{
    public interface IAddressService
    {
        Task<ApiResponse<AddressDto>> AddAsync(int userId, AddAddressDto dto, CancellationToken ct = default);
        Task<ApiResponse<List<AddressDto>>> GetMyAsync(int userId, CancellationToken ct = default);
        Task<ApiResponse<string>> UpdateAsync(int userId, int addressId, AddAddressDto dto, CancellationToken ct = default);
        Task<ApiResponse<string>> DeleteAsync(int userId, int addressId, CancellationToken ct = default);
        Task<ApiResponse<string>> SetDefaultAsync(int userId, int addressId, CancellationToken ct = default);
    }
}
