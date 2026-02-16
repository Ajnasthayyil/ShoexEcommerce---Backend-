//using ShoexEcommerce.Application.Interfaces.Media;

//namespace ShoexEcommerce.Infrastructure.Services
//{
//    public class SmsService : ISmsService
//    {
//        public Task SendAsync(string toPhone, string message, CancellationToken ct = default)
//            => Task.CompletedTask;
//    }
//}


using ShoexEcommerce.Application.Interfaces.Media;

namespace ShoexEcommerce.Infrastructure.Services
{
    public class SmsService : ISmsService
    {
        public Task SendAsync(string phoneNumber, string message, CancellationToken ct = default)
        {
            // TODO: integrate real SMS provider (Twilio, etc.)
            // Temporary: just log to console so DI works and app runs
            Console.WriteLine($"[SMS] To: {phoneNumber} | Message: {message}");
            return Task.CompletedTask;
        }
    }
}