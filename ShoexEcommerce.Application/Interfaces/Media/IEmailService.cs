namespace ShoexEcommerce.Application.Interfaces.Media
{
    public interface IEmailService
    {
        Task SendAsync(string toEmail, string subject, string body, CancellationToken ct = default);
    }
}