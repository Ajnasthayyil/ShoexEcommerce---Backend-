namespace ShoexEcommerce.Application.DTOs.Auth
{
    public class LoginResponseDto
    {
        public string Message { get; set; } = "Login successful";
        public string AccessToken { get; set; } = null!;
        public string RefreshToken { get; set; } = null!;
    }
}
