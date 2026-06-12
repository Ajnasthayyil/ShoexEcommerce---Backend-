using ShoexEcommerce.Application.Common;
using ShoexEcommerce.Application.DTOs.User;

namespace ShoexEcommerce.Application.Interfaces.User
{
    public interface IAdminUserService
    {
        Task<ApiResponse<List<AdminUserListDto>>> GetAllAsync(CancellationToken ct = default);
        Task<ApiResponse<AdminUserDetailDto>> GetByIdAsync(int id, CancellationToken ct = default);

        Task<ApiResponse<string>> ToggleBlockAsync(int adminId, int userId, CancellationToken ct = default);

        Task<ApiResponse<string>> DeleteAsync(int id, CancellationToken ct = default);
    }
}
