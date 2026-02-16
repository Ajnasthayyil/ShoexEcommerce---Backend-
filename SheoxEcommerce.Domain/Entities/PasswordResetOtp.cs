using ShoexEcommerce.Domain.Common;

namespace ShoexEcommerce.Domain.Entities
{
    public class PasswordResetOtp : BaseEntity
    {
        public int UserId { get; set; }
        public User User { get; set; } = null!;

        public string OtpHash { get; set; } = null!;
        public string Channel { get; set; } = "email"; 

        public DateTime ExpiresAtUtc { get; set; }
        public DateTime? VerifiedAtUtc { get; set; }

        public int Attempts { get; set; } = 0;
        public int MaxAttempts { get; set; } = 5;

        public DateTime NextResendAllowedAtUtc { get; set; } 
        public bool IsUsed { get; set; } = false;

        public string? ResetTokenHash { get; set; } 
        public DateTime? ResetTokenExpiresAtUtc { get; set; }

        public bool IsExpired() => DateTime.UtcNow > ExpiresAtUtc;
        public bool CanResend() => DateTime.UtcNow >= NextResendAllowedAtUtc;
    }
}
