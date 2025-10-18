using Interfaces;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using MimeKit;


namespace Repository
{
    public class EmailService : IEmail
    {
        private static IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<bool> SendEmail(string to, string from, string subject, string htmlMessage)
        {
            Console.WriteLine(to,from,subject,htmlMessage);
            try
            {
                MimeMessage message = new MimeMessage();

                MailboxAddress fromAddress = new MailboxAddress("Administrator",
                from);
                message.From.Add(fromAddress);

                MailboxAddress toAddress = new MailboxAddress("User",
                to);
                message.To.Add(toAddress);

                message.Subject = subject;

                BodyBuilder bodyBuilder = new BodyBuilder();
                bodyBuilder.HtmlBody = htmlMessage;

                message.Body = bodyBuilder.ToMessageBody();

                SmtpClient client = new SmtpClient();
                client.Connect("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
                var email = _configuration["AppEmail"];
                var password = _configuration["AppPassword"];
                client.Authenticate(email, password);
                await client.SendAsync(message);
                await client.DisconnectAsync(true);
                client.Dispose();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
