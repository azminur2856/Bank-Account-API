using BLL.DTOs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Services
{
    public class EmailService
    {
        private readonly EmailSecretDTO secret;

        public EmailService(EmailSecretDTO secret) {
            this.secret = secret;
        }

        public void SendEmail(string to, string subject, string body)
        {
            var smtpClient = new SmtpClient(secret.Host, secret.Port)
            {
                Credentials = new NetworkCredential(secret.User, secret.AppPassword),
                EnableSsl = true
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(secret.User),
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };

            mailMessage.To.Add(to);
            smtpClient.Send(mailMessage);
        }
    }
}
