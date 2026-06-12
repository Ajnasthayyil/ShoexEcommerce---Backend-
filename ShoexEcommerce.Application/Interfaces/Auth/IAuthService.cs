using ShoexEcommerce.Application.Common;
using ShoexEcommerce.Application.DTOs.Auth;
using ShoexEcommerce.Application.DTOs.User;
namespace ShoexEcommerce.Application.Interfaces.Auth
{
    public interface IAuthService
    {
        Task<ApiResponse<string>> RegisterAsync(RegisterDto dto);
        Task<ApiResponse<LoginResponseDto>> LoginAsync(LoginDto dto);
        Task<ProfileDto> GetProfileAsync(int userId);
        Task<ApiResponse<RefreshTokenResponseDto>> RefreshTokenAsync(string refreshToken);
        Task<ApiResponse<string>> UpdateMyProfileAsync(int userId, UpdateProfileDto dto, CancellationToken ct = default);
        Task<ApiResponse<string>> LogoutAsync(string refreshToken);
        Task<ApiResponse<string>> ForgotPasswordAsync(ForgotPasswordDto dto, CancellationToken ct = default);
        Task<ApiResponse<string>> VerifyOtpAsync(VerifyOtpDto dto, CancellationToken ct = default);
        Task<ApiResponse<string>> ResetPasswordAsync(ResetPasswordDto dto, CancellationToken ct = default);
    }
}