using Comet.Web.App.Data;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MimeKit;
using MimeKit.Text;
using MailKit.Net.Smtp;
using System;
using MailKit.Security;

namespace Comet.Web.App.Services
{
    public class EmailService : IEmailSender
    {
        private EmailSettings mSettings;
        private ApplicationDbContext mDbContext;
        private ILogger<EmailService> mLogger;

        public EmailService(ILogger<EmailService> logger, IOptions<EmailSettings> settings, ApplicationDbContext dbContext)
        {
            mSettings = settings.Value;
            mDbContext = dbContext;
            mLogger = logger;
        }

        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(mSettings.FromEmail, mSettings.FromEmail));
            message.To.Add(new MailboxAddress(email, email));
            message.Subject = subject;
            message.Body = new TextPart(TextFormat.Html)
            {
                Text = htmlMessage
            };
            try
            {
                using var client = new SmtpClient();
                /**
                 * PLEASE REMOVE THE BELOW LINES IF USING A PROPER SSL CERTIFICATE
                 */
                client.ServerCertificateValidationCallback = (s, c, h, e) => true; //NOSONAR
                client.CheckCertificateRevocation = false; //NOSONAR

                await client.ConnectAsync(mSettings.PrimaryDomain, mSettings.PrimaryPort, SecureSocketOptions.Auto);
                await client.AuthenticateAsync(mSettings.UsernameEmail, mSettings.UsernamePassword);
                await client.SendAsync(message);
                await client.DisconnectAsync(true);
            }
            catch (Exception ex)
            {
                mLogger.LogError(ex.ToString());
            }
        }

        public class EmailSettings
        {
            public string PrimaryDomain { get; set; }
            public int PrimaryPort { get; set; }
            public string UsernameEmail { get; set; }
            public string UsernamePassword { get; set; }
            public string FromEmail { get; set; }
            public bool EnableSsl { get; set; }
        }
    }
}
