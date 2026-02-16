using System.ComponentModel.DataAnnotations;

namespace ShoexEcommerce.Application.DTOs.Auth
{
    public class VerifyOtpDto
    {
        [Required, EmailAddress]
        public string Email { get; set; } = null!;

        [Required, MinLength(4), MaxLength(8)]
        public string Otp { get; set; } = null!;
    }
}