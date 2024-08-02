using System.Threading.Tasks;
using SendGrid;
using SendGrid.Helpers.Mail;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;     
namespace Domain.Post
{
    public class Email
    {
        private readonly string _apiKey;
        private readonly ILogger<Email> _logger;

        public Email(IConfiguration configuration, ILogger<Email> logger)
        {
            _apiKey = configuration["SendGrid:ApiKey"];
            _logger = logger;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string message)
        {
            _logger.LogInformation($"Sending email to: {toEmail}");
            _logger.LogInformation($"Subject: {subject}");
            _logger.LogInformation($"Body: {message}");

            var client = new SendGridClient(_apiKey);
            var from = new EmailAddress("tsikhishvilimariam@gmail.com", "Mariam");
            var to = new EmailAddress(toEmail);
            var msg = MailHelper.CreateSingleEmail(from, to, subject, message, message);
            var response = await client.SendEmailAsync(msg);

            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation("Email sent successfully.");
            }
            else
            {
                _logger.LogError($"Failed to send email. Status code: {response.StatusCode}");
            }
        }
    }
}
