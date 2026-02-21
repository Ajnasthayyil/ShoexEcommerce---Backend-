using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShoexEcommerce.Application.Common;
using ShoexEcommerce.Application.DTOs.Auth;
using ShoexEcommerce.Application.DTOs.User;
using ShoexEcommerce.Application.Interfaces.Auth;
using System.Security.Claims;

namespace ShoexEcommerce.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _auth;

        public AuthController(IAuthService auth)
        {
            _auth = auth;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            var result = await _auth.RegisterAsync(dto);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            var res = await _auth.LoginAsync(dto);

            if (!res.IsSuccess)
                return StatusCode(res.StatusCode, res);

            Response.Cookies.Append("access_token", res.Data!.AccessToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Expires = DateTimeOffset.UtcNow.AddMinutes(15)
            });

            Response.Cookies.Append("refresh_token", res.Data.RefreshToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Expires = DateTimeOffset.UtcNow.AddDays(7)
            });

            return StatusCode(res.StatusCode, res);
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken()
        {
            var refreshToken = Request.Cookies["refresh_token"];
            if (string.IsNullOrWhiteSpace(refreshToken))
                return Unauthorized(ApiResponse<string>.Fail("Refresh token missing", 401));

            var result = await _auth.RefreshTokenAsync(refreshToken);

            if (!result.IsSuccess)
                return StatusCode(result.StatusCode, result);

            Response.Cookies.Append("access_token", result.Data!.AccessToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Expires = DateTimeOffset.UtcNow.AddMinutes(15)
            });

            Response.Cookies.Append("refresh_token", result.Data.RefreshToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Expires = DateTimeOffset.UtcNow.AddDays(7)
            });

            return StatusCode(result.StatusCode, result);
        }

        [Authorize]
        [HttpGet("my-profile")]
        public async Task<IActionResult> Me()
        {
            var userIdStr = User.FindFirstValue("userid");

            if (string.IsNullOrWhiteSpace(userIdStr) || !int.TryParse(userIdStr, out var userId))
                return Unauthorized(ApiResponse<string>.Fail("Invalid token (userid missing)", 401));

            var profile = await _auth.GetProfileAsync(userId);
            return Ok(profile);
        }

        [Authorize]
        [HttpPut("update-profile")]
        public async Task<IActionResult> UpdateProfile([FromForm] UpdateProfileDto dto, CancellationToken ct)
        {
            var userIdStr = User.FindFirstValue("userid");

            if (string.IsNullOrWhiteSpace(userIdStr) || !int.TryParse(userIdStr, out var userId))
                return Unauthorized(ApiResponse<string>.Fail("Login required", 401));

            var res = await _auth.UpdateMyProfileAsync(userId, dto, ct);
            return StatusCode(res.StatusCode, res);
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromForm] ForgotPasswordDto dto, CancellationToken ct)
        {
            var res = await _auth.ForgotPasswordAsync(dto, ct);
            return StatusCode(res.StatusCode, res);
        }

        [HttpPost("verify-otp")]
        public async Task<IActionResult> VerifyOtp([FromBody] VerifyOtpDto dto, CancellationToken ct)
        {
            var res = await _auth.VerifyOtpAsync(dto, ct);
            return StatusCode(res.StatusCode, res);
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto dto, CancellationToken ct)
        {
            var res = await _auth.ResetPasswordAsync(dto, ct);
            return StatusCode(res.StatusCode, res);
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            var refreshToken = Request.Cookies["refresh_token"];
            if (string.IsNullOrWhiteSpace(refreshToken))
                return Unauthorized(ApiResponse<string>.Fail("Refresh token missing", 401));

            var result = await _auth.LogoutAsync(refreshToken);

            Response.Cookies.Delete("access_token");
            Response.Cookies.Delete("refresh_token");

            return StatusCode(result.StatusCode, result);
        }
    }
}
