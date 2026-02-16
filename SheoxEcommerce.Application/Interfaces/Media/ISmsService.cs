namespace ShoexEcommerce.Application.Interfaces.Media
{
    public interface ISmsService
    {
        Task SendAsync(string toPhone, string message, CancellationToken ct = default);
    }
}   