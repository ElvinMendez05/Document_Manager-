
using Document_Manager.Application.Interfaces;
using Microsoft.Extensions.Configuration;
using System.Net;
using System.Net.Mail;

namespace Document_Manager.Application.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendAsync(string to, string subject, string body)
        {
            var smtpClient = new SmtpClient
            {
                Host = _configuration["Email:Smtp"]!,
                Port = int.Parse(_configuration["Email:Port"]!),
                EnableSsl = true,
                Credentials = new NetworkCredential(
                    _configuration["Email:Username"],
                    _configuration["Email:Password"]
                )
            };

            var mail = new MailMessage(
                _configuration["Email:From"]!,
                to,
                subject,
                body
            );

            await smtpClient.SendMailAsync(mail);
        }
    }
}
