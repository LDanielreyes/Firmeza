using System.Net;
using System.Net.Mail;

namespace FirmezaAPI.Services
{
    public class GmailEmailService : IEmailService
    {
        private readonly IConfiguration _configuration;

        public GmailEmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendEmailAsync(string to, string subject, string body)
        {
            var smtpSettings = _configuration.GetSection("SmtpSettings");
            var mail = "your-email@gmail.com"; // Should be in config
            var password = "your-app-password"; // Should be in config

            var client = new SmtpClient("smtp.gmail.com", 587)
            {
                EnableSsl = true,
                Credentials = new NetworkCredential(smtpSettings["Username"], smtpSettings["Password"])
            };

            var mailMessage = new MailMessage(from: smtpSettings["Username"]!, to: to, subject: subject, body: body)
            {
                IsBodyHtml = true
            };

            await client.SendMailAsync(mailMessage);
        }
    }
}
