using BLL.DTOs;
using BLL.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace BLL.Services
{
    internal class EmailService : IEmailService
    {
        private readonly SecretSettingsDTO secret;

        public EmailService()
        {
            string path = Path.Combine(AppContext.BaseDirectory, "secretsettings.json");

            if (!File.Exists(path))
            {
                throw new FileNotFoundException("The configuration file 'secretsettings.json' was not found.");
            }

            var json = File.ReadAllText(path);

            var jsonDoc = JsonDocument.Parse(json);
            var secretSettingsJeson = jsonDoc.RootElement.GetProperty("SecretSettings").GetRawText();
            secret = JsonSerializer.Deserialize<SecretSettingsDTO>(secretSettingsJeson);
        }
        public void SendEmail(string to, string subject, string body)
        {
            var smtpClient = new SmtpClient(secret.Host)
            {
                Port = secret.Port,
                Credentials = new NetworkCredential(secret.FromEmail, secret.AppPassword),
                EnableSsl = true
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(secret.FromEmail),
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };

            mailMessage.To.Add(to);
            smtpClient.Send(mailMessage);
        }
    }
}
