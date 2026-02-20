using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using ShoexEcommerce.Application.Common;
using ShoexEcommerce.Application.DTOs.Auth;
using ShoexEcommerce.Application.DTOs.User;
using ShoexEcommerce.Application.Interfaces.Auth;
using ShoexEcommerce.Application.Interfaces.Media;
using ShoexEcommerce.Domain.Entities;
using ShoexEcommerce.Infrastructure.Data;
using ShoexEcommerce.Infrastructure.Services;

namespace ShoexEcommerce.Infrastructure.Security
{
    public class AuthService : IAuthService
    {
        private readonly AppDbContext _db;
        private readonly TokenService _tokenService;
        private readonly IEmailService _email;
        private readonly ISmsService _sms;


        public AuthService(AppDbContext db, TokenService tokenService, IEmailService email, ISmsService sms)
        {
            _db = db;
            _tokenService = tokenService;
            _email = email;
            _sms = sms;
        }



        // Normalizers 

        private static string NormalizeEmail(string? email)
            => (email ?? string.Empty).Trim().ToLowerInvariant();

        private static string NormalizeUsername(string? username)
            => (username ?? string.Empty).Trim().ToLowerInvariant();

        private static string NormalizeMobile(string? mobile)
            => (mobile ?? string.Empty).Trim();

        private static string NormalizeFullName(string? name)
            => (name ?? string.Empty).Trim();

        private async Task<string> GetRoleNameAsync(int roleId, CancellationToken ct = default)
        {
            var roleName = await _db.Roles
                .AsNoTracking()
                .Where(r => r.Id == roleId)
                .Select(r => r.Name)
                .FirstOrDefaultAsync(ct);

            return string.IsNullOrWhiteSpace(roleName) ? "User" : roleName!;
        }
        //register 
        public async Task<ApiResponse<string>> RegisterAsync(RegisterDto dto)
        {
            var email = dto.Email.Trim().ToLowerInvariant();

            if (await _db.Users.AnyAsync(u => u.Email == email))
                return ApiResponse<string>.Fail("Email already exists", 409);

            var userRoleId = await _db.Roles
                .Where(r => r.Name == "User")
                .Select(r => r.Id)
                .FirstOrDefaultAsync();

            if (userRoleId == 0)
                return ApiResponse<string>.Fail("Role 'User' not found", 500);

            var user = new User
            {
                FullName = dto.FullName.Trim(),
                Username = dto.Username.Trim(),
                Email = email,
                MobileNumber = dto.MobileNumber.Trim(),
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                RoleId = userRoleId,
                IsActive = true
            };

            _db.Users.Add(user);

            try
            {
                await _db.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                var msg = ex.InnerException?.Message ?? ex.Message;

                if (msg.Contains("IX_Users_Email", StringComparison.OrdinalIgnoreCase) ||
                    msg.Contains("Email", StringComparison.OrdinalIgnoreCase))
                    return ApiResponse<string>.Fail("Email already exists", 409);

                return ApiResponse<string>.Fail("Database error while registering user", 500);
            }


            return ApiResponse<string>.Success(null, "Registered successfully", 201);
        }


        // Login
        public async Task<ApiResponse<LoginResponseDto>> LoginAsync(LoginDto dto)
        {
            var username = NormalizeUsername(dto.Username);

            var user = await _db.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Username == username && u.IsActive);

            if (user == null)
                return ApiResponse<LoginResponseDto>.Fail("Invalid username or password", 401);

            var ok = BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash);
            if (!ok)
                return ApiResponse<LoginResponseDto>.Fail("Invalid username or password", 401);

            var roleName = await GetRoleNameAsync(user.RoleId);
            var accessToken = _tokenService.CreateAccessToken(user, roleName);

            var refreshToken = _tokenService.GenerateRefreshToken();
            var refreshExpiry = _tokenService.GetRefreshTokenExpiryUtc();

            _db.RefreshTokens.Add(new RefreshToken
            {
                Token = refreshToken,
                ExpiresOn = refreshExpiry,
                IsRevoked = false,
                UserId = user.Id
            });

            await _db.SaveChangesAsync();

            var data = new LoginResponseDto
            {
                //Message = "Login successful",
                AccessToken = accessToken,
                RefreshToken = refreshToken
            };

            return ApiResponse<LoginResponseDto>.Success(data, "Login successful", 200);
        }

        // Profile
        
        public async Task<ProfileDto> GetProfileAsync(int userId)
        {
            var user = await _db.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == userId);

            if (user == null)
                throw new Exception("User not found");

            return new ProfileDto
            {
                Id = user.Id,
                FullName = user.FullName,
                Username = user.Username,
                Email = user.Email,
                MobileNumber = user.MobileNumber,
                RoleId = user.RoleId,
                IsActive = user.IsActive,
                CreatedOn = user.CreatedOn
            };
        }

        // Refresh Token
        public async Task<ApiResponse<RefreshTokenResponseDto>> RefreshTokenAsync(string refreshToken)
        {
            if (string.IsNullOrWhiteSpace(refreshToken))
                return ApiResponse<RefreshTokenResponseDto>.Fail("Refresh token missing", 401);

            var tokenEntity = await _db.RefreshTokens
                .Include(x => x.User)
                .FirstOrDefaultAsync(x => x.Token == refreshToken);

            if (tokenEntity == null)
                return ApiResponse<RefreshTokenResponseDto>.Fail("Invalid refresh token", 401);

            if (tokenEntity.IsRevoked)
                return ApiResponse<RefreshTokenResponseDto>.Fail("Refresh token revoked", 401);

            if (tokenEntity.ExpiresOn <= DateTime.UtcNow)
                return ApiResponse<RefreshTokenResponseDto>.Fail("Refresh token expired", 401);

            var user = tokenEntity.User!;
            if (!user.IsActive)
                return ApiResponse<RefreshTokenResponseDto>.Fail("User is inactive", 403);

            // revoke old token
            tokenEntity.IsRevoked = true;

            // new refresh token
            var newRefreshToken = _tokenService.GenerateRefreshToken();
            var newRefreshExpiry = _tokenService.GetRefreshTokenExpiryUtc();

            _db.RefreshTokens.Add(new RefreshToken
            {
                Token = newRefreshToken,
                ExpiresOn = newRefreshExpiry,
                UserId = user.Id,
                IsRevoked = false
            });

            var roleName = await GetRoleNameAsync(user.RoleId);
            var newAccessToken = _tokenService.CreateAccessToken(user, roleName);

            await _db.SaveChangesAsync();

            return ApiResponse<RefreshTokenResponseDto>.Success(
                new RefreshTokenResponseDto
                {
                    AccessToken = newAccessToken,
                    RefreshToken = newRefreshToken
                },
                "Token refreshed successfully",
                200
            );
        }


        // Logout

        public async Task<ApiResponse<string>> LogoutAsync(string refreshToken)
        {
            if (string.IsNullOrWhiteSpace(refreshToken))
                return ApiResponse<string>.Fail("Refresh token missing", 400);

            var token = await _db.RefreshTokens
                .FirstOrDefaultAsync(x => x.Token == refreshToken && !x.IsRevoked);

            if (token == null)
                return ApiResponse<string>.Success(null, "Already logged out", 200);

            token.IsRevoked = true;
            await _db.SaveChangesAsync();

            return ApiResponse<string>.Success(null, "Logged out successfully", 200);
        }

        // Update Profile 

        public async Task<ApiResponse<string>> UpdateMyProfileAsync(
            int userId,
            UpdateProfileDto dto,
            CancellationToken ct = default)
        {
            var user = await _db.Users.FirstOrDefaultAsync(x => x.Id == userId, ct);
            if (user == null)
                return ApiResponse<string>.Fail("User not found", 404);

            user.FullName = NormalizeFullName(dto.FullName);
            user.Email = NormalizeEmail(dto.Email);
            user.MobileNumber = NormalizeMobile(dto.MobileNumber);

            await _db.SaveChangesAsync(ct);

            return ApiResponse<string>.Success(null, "Profile updated successfully", 200);
        }

        public async Task<ApiResponse<string>> ForgotPasswordAsync(ForgotPasswordDto dto, CancellationToken ct)
        {
            var email = NormalizeEmail(dto.Email);
            var channel = (dto.Channel ?? "email").Trim().ToLowerInvariant();

            if (channel is not ("email" or "sms" or "both"))
                return ApiResponse<string>.Fail("Invalid channel. Use: email | sms | both", 400);

            var user = await _db.Users
                .FirstOrDefaultAsync(x => x.Email == email && x.IsActive, ct);

            if (user == null)
                return ApiResponse<string>.Fail("Email does not exist", 400);


            var oldOtps = await _db.PasswordResetOtps
                .Where(x => x.UserId == user.Id && !x.IsUsed && x.VerifiedAtUtc == null)
                .ToListAsync(ct);

            if (oldOtps.Count > 0)
                _db.PasswordResetOtps.RemoveRange(oldOtps);

            var otp = OtpHelper.GenerateOtp();
            var otpHash = OtpHelper.Sha256($"{user.Id}:{otp}");

            var resetOtp = new PasswordResetOtp
            {
                UserId = user.Id,
                OtpHash = otpHash,
                Channel = channel,
                ExpiresAtUtc = DateTime.UtcNow.AddMinutes(5),
                NextResendAllowedAtUtc = DateTime.UtcNow.AddSeconds(60),
                MaxAttempts = 5,
                Attempts = 0,
                IsUsed = false
            };

            _db.PasswordResetOtps.Add(resetOtp);
            await _db.SaveChangesAsync(ct);

            if (channel == "email" || channel == "both")
            {
                await _email.SendAsync(
                    user.Email,
                    "Shoex Password Reset OTP",
                    $"Your OTP is {otp}",
                    ct
                );
            }

            if ((channel == "sms" || channel == "both"))
            {
                if (string.IsNullOrWhiteSpace(user.MobileNumber))
                    return ApiResponse<string>.Fail("Mobile number not found for this account", 400);

                await _sms.SendAsync(user.MobileNumber, $"Your OTP is {otp}", ct);
            }

            return ApiResponse<string>.Success(null, "OTP sent successfully", 200);
        }



        public async Task<ApiResponse<string>> VerifyOtpAsync(VerifyOtpDto dto, CancellationToken ct = default)
        {
            var user = await _db.Users.AsNoTracking()
                .FirstOrDefaultAsync(x => x.Email == dto.Email && x.IsActive, ct);

            // same security response
            if (user == null)
                return ApiResponse<string>.Fail("Invalid OTP.", 400);

            var otpRow = await _db.PasswordResetOtps
                .Where(x => x.UserId == user.Id && !x.IsUsed && x.VerifiedAtUtc == null)
                .OrderByDescending(x => x.CreatedOn)
                .FirstOrDefaultAsync(ct);

            if (otpRow == null)
                return ApiResponse<string>.Fail("OTP not found. Please request again.", 400);

            if (otpRow.IsExpired())
                return ApiResponse<string>.Fail("OTP expired. Please request again.", 400);

            if (otpRow.Attempts >= otpRow.MaxAttempts)
                return ApiResponse<string>.Fail("Too many attempts. Please request new OTP.", 429);

            otpRow.Attempts++;

            var incomingHash = OtpHelper.Sha256($"{user.Id}:{dto.Otp}");
            if (!string.Equals(incomingHash, otpRow.OtpHash, StringComparison.Ordinal))
            {
                await _db.SaveChangesAsync(ct);
                return ApiResponse<string>.Fail("Invalid OTP.", 400);
            }

            // OTP verified -> create reset token (return to client)
            var resetToken = OtpHelper.GenerateResetToken();
            otpRow.VerifiedAtUtc = DateTime.UtcNow;
            otpRow.ResetTokenHash = OtpHelper.Sha256($"{user.Id}:{resetToken}");
            otpRow.ResetTokenExpiresAtUtc = DateTime.UtcNow.AddMinutes(10);

            await _db.SaveChangesAsync(ct);

            return ApiResponse<string>.Success(resetToken, "OTP verified.");
        }

        public async Task<ApiResponse<string>> ResetPasswordAsync(ResetPasswordDto dto, CancellationToken ct = default)
        {
            var email = NormalizeEmail(dto.Email);

            //  Email must exist
            var user = await _db.Users
                .FirstOrDefaultAsync(x => x.Email == email && x.IsActive, ct);

            if (user == null)
                return ApiResponse<string>.Fail("Email does not exist", 400);

            //  Get latest verified OTP row
            var otpRow = await _db.PasswordResetOtps
                .Where(x => x.UserId == user.Id && !x.IsUsed && x.VerifiedAtUtc != null)
                .OrderByDescending(x => x.CreatedOn)
                .FirstOrDefaultAsync(ct);

            if (otpRow == null)
                return ApiResponse<string>.Fail("OTP not verified. Please verify OTP first.", 400);

            //  Check OTP again 
            if (otpRow.IsExpired())
                return ApiResponse<string>.Fail("OTP expired. Please request again.", 400);

            if (otpRow.Attempts >= otpRow.MaxAttempts)
                return ApiResponse<string>.Fail("Too many attempts. Please request new OTP.", 429);

            otpRow.Attempts++;

            var incomingOtpHash = OtpHelper.Sha256($"{user.Id}:{dto.Otp}");
            if (!string.Equals(incomingOtpHash, otpRow.OtpHash, StringComparison.Ordinal))
            {
                await _db.SaveChangesAsync(ct);
                return ApiResponse<string>.Fail("Invalid OTP.", 400);
            }

            //  Reset token validation
            if (string.IsNullOrWhiteSpace(otpRow.ResetTokenHash))
                return ApiResponse<string>.Fail("Reset token missing. Please verify OTP again.", 400);

            if (otpRow.ResetTokenExpiresAtUtc == null || DateTime.UtcNow > otpRow.ResetTokenExpiresAtUtc.Value)
                return ApiResponse<string>.Fail("Reset session expired. Please try again.", 400);

            var incomingResetHash = OtpHelper.Sha256($"{user.Id}:{dto.ResetToken}");
            if (!string.Equals(incomingResetHash, otpRow.ResetTokenHash, StringComparison.Ordinal))
                return ApiResponse<string>.Fail("Invalid reset token.", 400);

            //  Password strength: already validated by DTO
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);

            // revoke refresh tokens
            var refreshTokens = await _db.RefreshTokens
                .Where(x => x.UserId == user.Id && !x.IsRevoked)
                .ToListAsync(ct);

            foreach (var rt in refreshTokens)
                rt.IsRevoked = true;

            otpRow.IsUsed = true; // mark this reset flow as completed
            await _db.SaveChangesAsync(ct);

            return ApiResponse<string>.Success(null, "Password reset successful. Please login again.", 200);
        }


    }
}
