using System.Security.Cryptography;
using System.Text;

namespace ShoexEcommerce.Infrastructure.Security
{
    public static class OtpHelper
    {
        // 6-digit numeric OTP
        public static string GenerateOtp(int length = 6)
        {
            var bytes = RandomNumberGenerator.GetBytes(length);
            var sb = new StringBuilder(length);

            for (int i = 0; i < length; i++)
                sb.Append((bytes[i] % 10).ToString());

            return sb.ToString();
        }

        // SHA256 hash (used for OTP + ResetToken hashing)
        public static string Sha256(string input)
        {
            using var sha = SHA256.Create();
            var hash = sha.ComputeHash(Encoding.UTF8.GetBytes(input));
            return Convert.ToHexString(hash); // Hex string
        }

        // Random reset token (return to client after OTP verify)
        public static string GenerateResetToken()
        {
            return Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));
        }
    }
}
