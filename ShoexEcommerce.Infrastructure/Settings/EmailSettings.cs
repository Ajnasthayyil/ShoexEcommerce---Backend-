namespace ShoexEcommerce.Infrastructure.Settings
{
    public class EmailSettings
    {
        public string Host { get; set; } = null!;
        public int Port { get; set; }
        public string Username { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string From { get; set; } = null!;
        public string FromName { get; set; } = "Shoex Ecommerce";
    }
}