using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using ShoexEcommerce.Domain.Entities;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace ShoexEcommerce.Infrastructure.Security
{
    public class TokenService
    {
        private readonly IConfiguration _config;

        public TokenService(IConfiguration config)
        {
            _config = config;
        }

        public string CreateAccessToken(User user, string roleName)
        {
            var jwtKey = _config["Jwt:Key"]!;
            var minutes = int.TryParse(_config["Jwt:AccessTokenMinutes"], out var m) ? m : 15;

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var payload = new JwtPayload
            {
                { "userid", user.Id },              // ✅ number
                { "username", user.Username },      // string
                { "roleid", user.RoleId },          // ✅ number
                { ClaimTypes.Role, roleName },      // string
                { JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString() },
                { JwtRegisteredClaimNames.Iss, _config["Jwt:Issuer"] },
                { JwtRegisteredClaimNames.Aud, _config["Jwt:Audience"] },
                { JwtRegisteredClaimNames.Exp, new DateTimeOffset(DateTime.UtcNow.AddMinutes(minutes)).ToUnixTimeSeconds() }
            };

            var token = new JwtSecurityToken(new JwtHeader(creds), payload);
            return new JwtSecurityTokenHandler().WriteToken(token);
        }


        // ✅ Refresh token random string
        public string GenerateRefreshToken()
        {
            var bytes = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(bytes);
            return Convert.ToBase64String(bytes);
        }

 
        public DateTime GetRefreshTokenExpiryUtc()
        {
            var daysStr = _config["Jwt:RefreshTokenDays"];
            var days = int.TryParse(daysStr, out var d) ? d : 7;

            return DateTime.UtcNow.AddDays(days);
        }
    }
}
