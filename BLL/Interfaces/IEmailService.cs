using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Interfaces
{
    public interface IEmailService
    {
        void SendActivationEmail(string to, string fullName, string activationLink);
        void SendWelcomeEmail(string to, string fullName, string dashboardLink);
        void SendTransactionNotificationEmail(string to, string fullName, string type, string amount, string date, string reference);
        void SendPasswordResetOtpEmail(string to, string fullName, string otp);
        void SendNotificationEmail(string to, string fullName, string subject, string messageBody);
        void SendCustomEmail(string to, string fullName, string subject, string headline, string messageBody);
        void SendCustomButtonEmail(string to, string fullName, string subject, string headline, string messageBody, string buttonText, string buttonLink);
        void SendNewAccountDetailsEmail(string to, string fullName, string accountNumber, string accountType, string openingBalance, string openingTime);
        void SendVerifyEmail(string to, string fullName, string verificationLink);
        void SendAccountActivatedEmail(string to, string fullName);
        void SendAccountDeactivatedEmail(string to, string fullName);
    }
}
