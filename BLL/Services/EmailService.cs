using BLL.DTOs;
using BLL.Interfaces;
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
    internal class EmailService : IEmailService
    {
        private readonly EmailSecretDTO secret;

        public EmailService(EmailSecretDTO secret) {
            this.secret = secret;
        }

        private void SendEmail(string to, string subject, string body)
        {
            var smtpClient = new SmtpClient(secret.Host, secret.Port)
            {
                Credentials = new NetworkCredential(secret.User, secret.AppPassword),
                EnableSsl = true
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(secret.User, "ART Bank PLC"),
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };

            mailMessage.To.Add(to);
            smtpClient.Send(mailMessage);
        }

        // 1. Account Activation Email
        public void SendActivationEmail(string to, string fullName, string activationLink)
        {
            string subject = "Activate Your Account";
            string body = $@"
            <div style='font-family: Arial, sans-serif; background-color: #f4f6f8; padding: 20px;'>
              <div style='max-width: 600px; margin: auto; background: #ffffff; border-radius: 8px; box-shadow: 0 2px 6px rgba(0,0,0,0.1);'>
                <div style='background-color: #0d6efd; padding: 20px; text-align: center; border-radius: 8px 8px 0 0;'>
                  <h2 style='color: #fff;'>Activate Your Account</h2>
                </div>
                <div style='padding: 20px;'>
                  <p>Hello <strong>{fullName}</strong>,</p>
                  <p>Thank you for creating an account with <strong>ART Bank PLC</strong>. Please confirm your email address by clicking below:</p>
                  <p style='text-align: center;'>
                    <a href='{activationLink}' style='background: #0d6efd; color: #fff; padding: 12px 24px; text-decoration: none; border-radius: 5px;'>Activate Account</a>
                  </p>
                  <p><strong>Note:</strong> This link will expire in <strong>24 hours</strong>.</p>
                  <p>If you didn’t create this account, please ignore this email.</p>
                  <p style='margin-top: 40px;'>Regards,<br><strong>ART Bank PLC Team</strong></p>
                </div>
                {Footer()}
              </div>
            </div>";
            SendEmail(to, subject, body);
        }

        // 2. Welcome Email
        public void SendWelcomeEmail(string to, string fullName, string dashboardLink)
        {
            string subject = "Welcome to ART Bank PLC!";
            string body = $@"
            <div style='font-family: Arial, sans-serif; background-color: #f4f6f8; padding: 20px;'>
              <div style='max-width: 600px; margin: auto; background: #ffffff; border-radius: 8px; box-shadow: 0 2px 6px rgba(0,0,0,0.1);'>
                <div style='background-color: #198754; padding: 20px; text-align: center; border-radius: 8px 8px 0 0;'>
                  <h2 style='color: #fff;'>Welcome to ART Bank PLC!</h2>
                </div>
                <div style='padding: 20px;'>
                  <p>Hello <strong>{fullName}</strong>,</p>
                  <p>We’re delighted to have you onboard! Start exploring your account and services today.</p>
                  <p style='text-align: center;'>
                    <a href='{dashboardLink}' style='background: #198754; color: #fff; padding: 12px 24px; text-decoration: none; border-radius: 5px;'>Go to Dashboard</a>
                  </p>
                  <p>We look forward to serving you.<br><strong>ART Bank PLC Team</strong></p>
                </div>
                {Footer()}
              </div>
            </div>";
            SendEmail(to, subject, body);
        }

        // 3. Transaction Notification
        public void SendTransactionNotificationEmail(string to, string fullName, string type, string amount, string date, string reference)
        {
            string subject = $"Transaction Alert - {type}";
            string body = $@"
            <div style='font-family: Arial, sans-serif; background-color: #f4f6f8; padding: 20px;'>
              <div style='max-width: 600px; margin: auto; background: #ffffff; border-radius: 8px; box-shadow: 0 2px 6px rgba(0,0,0,0.1);'>
                <div style='background-color: #0dcaf0; padding: 20px; text-align: center; border-radius: 8px 8px 0 0;'>
                  <h2 style='color: #fff;'>Transaction Alert</h2>
                </div>
                <div style='padding: 20px;'>
                  <p>Hello <strong>{fullName}</strong>,</p>
                  <p>A recent transaction has been processed on your account:</p>
                  <ul>
                    <li><strong>Type:</strong> {type}</li>
                    <li><strong>Amount:</strong> {amount}</li>
                    <li><strong>Date:</strong> {date}</li>
                    <li><strong>Reference:</strong> {reference}</li>
                  </ul>
                  <p>If this wasn’t you, please contact our support team immediately.</p>
                  <p><strong>ART Bank PLC Security Team</strong></p>
                </div>
                {Footer()}
              </div>
            </div>";
            SendEmail(to, subject, body);
        }

        // 4. Password Reset (OTP)
        public void SendPasswordResetOtpEmail(string to, string fullName, string otp)
        {
            string subject = "Password Reset OTP";
            string body = $@"
            <div style='font-family: Arial, sans-serif; background-color: #f4f6f8; padding: 20px;'>
              <div style='max-width: 600px; margin: auto; background: #ffffff; border-radius: 8px; box-shadow: 0 2px 6px rgba(0,0,0,0.1);'>
                <div style='background-color: #dc3545; padding: 20px; text-align: center; border-radius: 8px 8px 0 0;'>
                  <h2 style='color: #fff;'>Password Reset Request</h2>
                </div>
                <div style='padding: 20px;'>
                  <p>Hello <strong>{fullName}</strong>,</p>
                  <p>We received a request to reset your password. Use the OTP below:</p>
                  <p style='text-align: center; font-size: 24px; font-weight: bold; background: #f8f9fa; padding: 10px; border-radius: 5px;'>{otp}</p>
                  <p>This OTP will expire in <strong>5 minutes</strong>. If you didn’t request this, please ignore this email.</p>
                  <p><strong>ART Bank PLC Security Team</strong></p>
                </div>
                {Footer()}
              </div>
            </div>";
            SendEmail(to, subject, body);
        }

        // 5. Generic Notification
        public void SendNotificationEmail(string to, string fullName, string subject, string messageBody)
        {
            string body = $@"
            <div style='font-family: Arial, sans-serif; background-color: #f4f6f8; padding: 20px;'>
              <div style='max-width: 600px; margin: auto; background: #ffffff; border-radius: 8px; box-shadow: 0 2px 6px rgba(0,0,0,0.1);'>
                <div style='background-color: #6c757d; padding: 20px; text-align: center; border-radius: 8px 8px 0 0;'>
                  <h2 style='color: #fff;'>Notification</h2>
                </div>
                <div style='padding: 20px;'>
                  <p>Hello <strong>{fullName}</strong>,</p>
                  <p>{messageBody}</p>
                  <p>Regards,<br><strong>ART Bank PLC Team</strong></p>
                </div>
                {Footer()}
              </div>
            </div>";
            SendEmail(to, subject, body);
        }

        // 6. Fully Custom Email
        public void SendCustomEmail(string to, string fullName, string subject, string headline, string messageBody)
        {
            string body = $@"
            <div style='font-family: Arial, sans-serif; background-color: #f4f6f8; padding: 20px;'>
              <div style='max-width: 650px; margin: auto; background: #ffffff; border-radius: 8px; box-shadow: 0 4px 12px rgba(0,0,0,0.15);'>
                <div style='background: linear-gradient(90deg, #0d6efd, #198754); padding: 30px; text-align: center; border-radius: 8px 8px 0 0;'>
                  <h1 style='color: #fff; margin: 0;'>{headline}</h1>
                </div>
                <div style='padding: 25px;'>
                  <p>Hello <strong>{fullName}</strong>,</p>
                  <p style='font-size: 15px; line-height: 1.6;'>{messageBody}</p>
                  <p style='margin-top: 30px;'>Best regards,<br><strong>ART Bank PLC Team</strong></p>
                </div>
                {Footer()}
              </div>
            </div>";
            SendEmail(to, subject, body);
        }

        // 7. Fully Custom Template (with Button)
        public void SendCustomButtonEmail(
            string to,
            string fullName,
            string subject,
            string headline,
            string messageBody,
            string buttonText,
            string buttonLink)
        {
            string body = $@"
            <div style='font-family: Arial, sans-serif; background-color: #f4f6f8; padding: 20px;'>
              <div style='max-width: 650px; margin: auto; background: #ffffff; border-radius: 8px; box-shadow: 0 4px 12px rgba(0,0,0,0.15);'>

                <!-- Header / Banner -->
                <div style='background: linear-gradient(90deg, #0d6efd, #198754); padding: 30px; text-align: center; border-radius: 8px 8px 0 0;'>
                  <h1 style='color: #fff; margin: 0;'>{headline}</h1>
                </div>

                <!-- Body -->
                <div style='padding: 25px;'>
                  <p>Hello <strong>{fullName}</strong>,</p>
                  <p style='font-size: 15px; line-height: 1.6;'>{messageBody}</p>
                  <p style='text-align: center; margin: 30px 0;'>
                    <a href='{buttonLink}' style='background: #0d6efd; color: #fff; font-size: 16px; padding: 14px 28px; text-decoration: none; border-radius: 6px; font-weight: bold;'>{buttonText}</a>
                  </p>
                  <p>If you have any questions, please contact our <a href='mailto:support@artbank.com' style='color:#0d6efd;'>support team</a>.</p>
                  <p style='margin-top: 20px;'>Best regards,<br><strong>ART Bank PLC Team</strong></p>
                </div>

                <!-- Divider -->
                <hr style='border: none; border-top: 1px solid #e0e0e0; margin: 0;' />

                {Footer()}
              </div>
            </div>";
            SendEmail(to, subject, body);
        }

        // 8. New Account Creation Details
        public void SendNewAccountDetailsEmail(string to, string fullName, string accountNumber, string accountType, string openingBalance, string openingTime)
        {
            string subject = "Your New ART Bank PLC Account Has Been Created";
            string body = $@"
            <div style='font-family: Arial, sans-serif; background-color: #f4f6f8; padding: 20px;'>
              <div style='max-width: 650px; margin: auto; background: #ffffff; border-radius: 8px; box-shadow: 0 4px 12px rgba(0,0,0,0.15);'>
                <div style='background-color: #0d6efd; padding: 25px; text-align: center; border-radius: 8px 8px 0 0;'>
                  <h2 style='color: #fff; margin: 0;'>Welcome to ART Bank PLC</h2>
                </div>
                <div style='padding: 25px;'>
                  <p>Hello <strong>{fullName}</strong>,</p>
                  <p>Your new account has been successfully created. Here are the details:</p>
                  <table style='width: 100%; border-collapse: collapse; margin-top: 15px;'>
                    <tr style='background: #f8f9fa;'>
                      <td style='padding: 10px; border: 1px solid #dee2e6;'><strong>Account Number</strong></td>
                      <td style='padding: 10px; border: 1px solid #dee2e6;'>{accountNumber}</td>
                    </tr>
                    <tr>
                      <td style='padding: 10px; border: 1px solid #dee2e6;'><strong>Account Type</strong></td>
                      <td style='padding: 10px; border: 1px solid #dee2e6;'>{accountType}</td>
                    </tr>
                    <tr style='background: #f8f9fa;'>
                      <td style='padding: 10px; border: 1px solid #dee2e6;'><strong>Opening Balance</strong></td>
                      <td style='padding: 10px; border: 1px solid #dee2e6;'>{openingBalance}</td>
                    </tr>
                    <tr>
                      <td style='padding: 10px; border: 1px solid #dee2e6;'><strong>Opening Time</strong></td>
                      <td style='padding: 10px; border: 1px solid #dee2e6;'>{openingTime}</td>
                    </tr>
                  </table>
                  <p style='margin-top: 25px;'>You can now log in to your online banking portal to manage your account and explore our services.</p>
                  <p style='margin-top: 20px;'>Thank you for choosing <strong>ART Bank PLC</strong>.<br>We look forward to serving you.</p>
                </div>
                <div style='padding: 15px; font-size: 12px; background: #fff3cd; color: #856404; border-top: 1px solid #ffeeba;'>
                  ⚠️ <strong>Security Tip:</strong> Please do not share your account details, PIN, or password with anyone. ART Bank PLC will never ask for such information via email or phone.
                </div>
                {Footer()}
              </div>
            </div>";
            SendEmail(to, subject, body);
        }

        // Shared Footer
        private string Footer()
        {
            return $@"
            <div style='background: #f8f9fa; text-align: center; padding: 15px; font-size: 12px; color: #6c757d; border-radius: 0 0 8px 8px;'>
              © {DateTime.Now.Year} ART Bank PLC. All rights reserved.<br />
              Head Office, Motijheel, Dhaka, Bangladesh
            </div>";
        }
    }
}