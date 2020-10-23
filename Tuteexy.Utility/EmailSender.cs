using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace Tuteexy.Utility
{
    public class EmailSender : IEmailSender
    {
        private readonly EmailOptions emailOptions;

        public EmailSender(IOptions<EmailOptions> options)
        {
            emailOptions = options.Value;
        }
        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            return Execute(subject, htmlMessage, email);
        }
        private Task Execute(string subject, string message, string email)
        {
            var smtpClient = new SmtpClient();

            smtpClient.EnableSsl = true;
            smtpClient.Host = emailOptions.Host;
            smtpClient.Port = emailOptions.Port;
            smtpClient.UseDefaultCredentials = false;
            smtpClient.Credentials = new NetworkCredential(emailOptions.From, emailOptions.Pvalue);

            MailMessage mailMessage = new MailMessage();
            mailMessage.From = new MailAddress(emailOptions.From, emailOptions.Alias);
            mailMessage.BodyEncoding = Encoding.UTF8;
            mailMessage.To.Add(email);
            mailMessage.Body = message;
            mailMessage.Subject = subject;
            mailMessage.IsBodyHtml = true;

            return smtpClient.SendMailAsync(mailMessage);
        }
    }
}
