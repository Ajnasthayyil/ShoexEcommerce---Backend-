using System.ComponentModel.DataAnnotations;

namespace ShoexEcommerce.Application.DTOs.Auth
{
    public class ResetPasswordDto
    {
        [Required, EmailAddress]
        public string Email { get; set; } = null!;

        // OTP required for reset step 
        [Required]
        [StringLength(6, MinimumLength = 4)]
        public string Otp { get; set; } = null!;

        // reset token returned from verify otp
        [Required]
        public string ResetToken { get; set; } = null!;

        // strong password validation
        [Required]
        [MinLength(8)]
        [RegularExpression(
            @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^A-Za-z\d]).{8,}$",
            ErrorMessage = "Password must be at least 8 chars and contain 1 uppercase, 1 lowercase, 1 digit, and 1 special character."
        )]
        public string NewPassword { get; set; } = null!;
    }
}
