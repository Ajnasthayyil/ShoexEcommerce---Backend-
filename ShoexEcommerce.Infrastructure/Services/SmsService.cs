using ShoexEcommerce.Application.Interfaces.Media;

namespace ShoexEcommerce.Infrastructure.Services
{
    public class SmsService : ISmsService
    {
        public Task SendAsync(string toPhone, string message, CancellationToken ct = default)
            => Task.CompletedTask;
    }
}
