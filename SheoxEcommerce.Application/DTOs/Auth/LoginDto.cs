using System.ComponentModel.DataAnnotations;

namespace ShoexEcommerce.Application.DTOs.Auth
{
    public class LoginDto
    {
        [Required]
        public string Username { get; set; } = null!;

        private string _password = string.Empty;

        [Required]
        public string Password
        {
            get => _password;
            set => _password = value?.Trim() ?? string.Empty;
        }
    }
}
