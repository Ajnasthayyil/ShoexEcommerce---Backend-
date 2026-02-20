using Microsoft.EntityFrameworkCore;
using ShoexEcommerce.Application.Common;
using ShoexEcommerce.Application.DTOs.User;
using ShoexEcommerce.Application.Interfaces.User;
using ShoexEcommerce.Infrastructure.Data;

namespace ShoexEcommerce.Infrastructure.Services
{
    public class AdminUserService : IAdminUserService
    {
        private readonly AppDbContext _db;

        public AdminUserService(AppDbContext db)
        {
            _db = db;
        }

        // Get all users
        public async Task<ApiResponse<List<AdminUserListDto>>> GetAllAsync(CancellationToken ct = default)
        {
            var users = await _db.Users
                .AsNoTracking()
                .Include(x => x.Role)
                .OrderByDescending(x => x.CreatedOn)
                .Select(x => new AdminUserListDto
                {
                    Id = x.Id,
                    FullName = x.FullName,
                    Email = x.Email,
                    MobileNumber = x.MobileNumber,
                    Username = x.Username,
                    RoleId = x.RoleId,
                    RoleName = x.Role != null ? x.Role.Name : "",
                    IsBlocked = x.IsBlocked,
                    IsActive = x.IsActive,
                    CreatedOn = x.CreatedOn
                })
                .ToListAsync(ct);

            return ApiResponse<List<AdminUserListDto>>.Success(users, "Users fetched", 200);
        }

        
        // Get user by id
        public async Task<ApiResponse<AdminUserDetailDto>> GetByIdAsync(int id, CancellationToken ct = default)
        {
            var user = await _db.Users
                .AsNoTracking()
                .Include(x => x.Role)
                .Where(x => x.Id == id)
                .Select(x => new AdminUserDetailDto
                {
                    Id = x.Id,
                    FullName = x.FullName,
                    Email = x.Email,
                    MobileNumber = x.MobileNumber,
                    Username = x.Username,
                    RoleId = x.RoleId,
                    RoleName = x.Role != null ? x.Role.Name : "",
                    IsBlocked = x.IsBlocked,
                    IsActive = x.IsActive,
                    CreatedOn = x.CreatedOn
                })
                .FirstOrDefaultAsync(ct);

            if (user == null)
                return ApiResponse<AdminUserDetailDto>.Fail("User not found", 404);

            return ApiResponse<AdminUserDetailDto>.Success(user, "User fetched", 200);
        }

        // Block / Unblock User
        public async Task<ApiResponse<string>> ToggleBlockAsync(int adminId, int userId, CancellationToken ct = default)
        {
            if (adminId == userId)
                return ApiResponse<string>.Fail("Admin cannot block own account", 400);

            var admin = await _db.Users
                .Include(x => x.Role)
                .FirstOrDefaultAsync(x => x.Id == adminId && x.IsActive, ct);

            if (admin == null || admin.Role?.Name != "Admin")
                return ApiResponse<string>.Fail("Unauthorized admin", 403);

            var user = await _db.Users
                .Include(x => x.Role)
                .Include(x => x.RefreshTokens)
                .FirstOrDefaultAsync(x => x.Id == userId, ct);

            if (user == null)
                return ApiResponse<string>.Fail("User not found", 404);

            if (user.Role?.Name == "Admin")
                return ApiResponse<string>.Fail("Admin cannot be blocked", 400);

            user.IsBlocked = !user.IsBlocked;
            user.ModifiedOn = DateTime.UtcNow;

            if (user.IsBlocked)
            {
                foreach (var t in user.RefreshTokens.Where(x => !x.IsRevoked))
                    t.IsRevoked = true;
            }

            await _db.SaveChangesAsync(ct);

            return ApiResponse<string>.Success(
                null,
                user.IsBlocked ? "User blocked successfully" : "User unblocked successfully",
                200);
        }


        // Delete user permanently
        public async Task<ApiResponse<string>> DeleteAsync(int id, CancellationToken ct = default)
        {
            var user = await _db.Users
                .Include(x => x.RefreshTokens)
                .FirstOrDefaultAsync(x => x.Id == id, ct);

            if (user == null)
                return ApiResponse<string>.Fail("User not found", 404);

            _db.RefreshTokens.RemoveRange(user.RefreshTokens);
            _db.Users.Remove(user);

            await _db.SaveChangesAsync(ct);

            return ApiResponse<string>.Success(null, "User deleted", 200);
        }
    }
}
