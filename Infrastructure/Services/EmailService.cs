using Application.Services;
using Infrastructure.Settings;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using MailKit.Net.Smtp;

namespace Infrastructure.Services
{
    public class EmailService : IEmailService
    {
        private readonly EmailSettings _settings;

        public EmailService(IOptions<EmailSettings> settings)
        {
            _settings = settings.Value;
        }

        public async Task SendEmailAsync(string to, string subject, string body)
        {
            await SendEmailAsync(new List<string> { to }, subject, body);
        }

        public async Task SendEmailAsync(List<string> to, string subject, string body)
        {
            var email = new MimeMessage();

            // From
            email.From.Add(new MailboxAddress(_settings.DisplayName, _settings.From));

            // To
            foreach (var recipient in to)
                email.To.Add(MailboxAddress.Parse(recipient));

            email.Subject = subject;

            // HTML body
            email.Body = new BodyBuilder { HtmlBody = body }.ToMessageBody();

            using var smtp = new SmtpClient();

            await smtp.ConnectAsync(_settings.Host, _settings.Port, SecureSocketOptions.StartTls);
            await smtp.AuthenticateAsync(_settings.From, _settings.Password);
            await smtp.SendAsync(email);
            await smtp.DisconnectAsync(true);
        }
    }

}
