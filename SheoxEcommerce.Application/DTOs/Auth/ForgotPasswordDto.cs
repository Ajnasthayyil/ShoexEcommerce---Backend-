using System.ComponentModel.DataAnnotations;

namespace ShoexEcommerce.Application.DTOs.Auth
{
    public class ForgotPasswordDto
    {
        [Required, EmailAddress]
        public string Email { get; set; } = null!;

        public string? Channel { get; set; } = "email";
    }
}
