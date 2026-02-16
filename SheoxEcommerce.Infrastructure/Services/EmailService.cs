using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Options;
using ShoexEcommerce.Application.Interfaces.Media;
using ShoexEcommerce.Infrastructure.Settings;

namespace ShoexEcommerce.Infrastructure.Services
{
    public class EmailService : IEmailService
    {
        private readonly EmailSettings _s;

        public EmailService(IOptions<EmailSettings> options)
        {
            _s = options.Value;
        }

        public async Task SendAsync(string toEmail, string subject, string body, CancellationToken ct = default)
        {
            using var smtp = new SmtpClient(_s.Host, _s.Port)
            {
                EnableSsl = true,
                Credentials = new NetworkCredential(_s.Username, _s.Password)
            };

            using var mail = new MailMessage
            {
                From = new MailAddress(_s.From, _s.FromName),
                Subject = subject,
                Body = body,
                IsBodyHtml = false
            };

            mail.To.Add(toEmail);
            await smtp.SendMailAsync(mail);
        }
    }
}