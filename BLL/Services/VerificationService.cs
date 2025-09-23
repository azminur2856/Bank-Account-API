using BLL.DTOs;
using BLL.Utility;
using DAL;
using DAL.EF.Tables;
using DAL.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Services
{
    public class VerificationService
    {
        public static Task<bool> SendEmailVerificationLink(UserDTO user, bool isRegistration = false)
        {
            var vToken = Helper.GenerateToken();

            var verification = new Verification
            {
                Code = vToken,
                Type = VerificationType.EmailVerification,
                IsUsed = false,
                CreatedAt = DateTime.UtcNow,
                ExpireAt = DateTime.UtcNow.AddHours(24),
                UserId = user.UserId
            };

            var isCreated = DataAccessFactory.VerificationData().Create(verification);

            if (!isCreated) return Task.FromResult(false);

            var verificationLink = string.Empty;

            if (isRegistration)
            {
                verificationLink = $"{ServiceFactory.ApiBaseUrl}/api/user/verifyaccount/{vToken}";
            }

            verificationLink = $"{ServiceFactory.ApiBaseUrl}/api/user/verifyemail/{vToken}";

            ServiceFactory.EmailService.SendActivationEmail(user.Email, user.FullName, verificationLink);

            var auditLog = new AuditLogDTO
            {
                UserId = user.UserId,
                Type = AuditLogType.EmailVerificationLinkSent,
                Details = $"Verification email sent to {user.Email} with token {vToken}",
            };

            return Task.FromResult(true);
        }

        public static bool VerifyAccount(string token)
        {
            var verification = DataAccessFactory.VerificationFeaturesData().GetByCodeAndType(token, VerificationType.EmailVerification);

            if (verification == null) return false;

            var userRepo = DataAccessFactory.UserData();
            var user = userRepo.Get(verification.UserId);

            if (user == null) return false;

            user.IsEmailVerified = true;
            user.IsActive = true;
            user.UpdatedAt = DateTime.Now;

            var isUserUpdated = userRepo.Update(user);

            if (isUserUpdated)
            {
                verification.IsUsed = true;
                DataAccessFactory.VerificationData().Update(verification);

                var auditLog = new AuditLogDTO
                {
                    UserId = user.UserId,
                    Type = AuditLogType.CompleateRegistrationActiveAccountAndVerifyEmail,
                    Details = $"User completed registration and activated account via email verification for {user.Email}.",
                };
                AuditLogService.LogActivity(auditLog);
            }

            ServiceFactory.EmailService.SendWelcomeEmail(user.Email, user.FullName, ServiceFactory.ApiBaseUrl);

            return isUserUpdated;
        }

        public static async Task<bool> SendPhoneVerificationOtp(string tokenKey)
        {
            var token = DataAccessFactory.TokenData().Get(tokenKey);
            if (token != null && token.ExpireAt == null)
            {
                var userRepo = DataAccessFactory.UserData();
                var user = userRepo.Get(token.UserId);

                if (user == null)
                {
                    throw new KeyNotFoundException("User not found.");
                }

                if (user.IsPhoneNumberVerified)
                {
                    throw new UnauthorizedAccessException("Phone number is already verified.");
                }

                var otp = Helper.GenerateOtp();

                var verification = new Verification
                {
                    Code = otp,
                    Type = VerificationType.PhoneVerification,
                    IsUsed = false,
                    CreatedAt = DateTime.UtcNow,
                    ExpireAt = DateTime.UtcNow.AddMinutes(5),
                    UserId = user.UserId
                };

                var isCreated = DataAccessFactory.VerificationData().Create(verification);

                if (!isCreated) return false;

                var smsMessage = $"Your phone verification code is: {otp}. It will expire in 5 minutes.";

                var smsSend = await ServiceFactory.SmsService.SendSMSAsync(user.PhoneNumber, smsMessage);

                if (smsSend) { 
                    var auditLog = new AuditLogDTO
                    {
                        UserId = user.UserId,
                        Type = AuditLogType.PhoneVerificationCodeSent,
                        Details = $"Phone verification OTP sent to {user.PhoneNumber} with code {otp}",
                    };
                    AuditLogService.LogActivity(auditLog);
                }
                return smsSend;
            }
            return false;
        }

        public static bool VerifyPhoneNumber(string tokenKey, string otp)
        {
            var token = DataAccessFactory.TokenData().Get(tokenKey);
            if (token != null && token.ExpireAt == null)
            {
                var userRepo = DataAccessFactory.UserData();
                var user = userRepo.Get(token.UserId);

                if (user == null)
                {
                    throw new KeyNotFoundException("User not found.");
                }

                var verification = DataAccessFactory.VerificationFeaturesData().GetByCodeAndType(otp, VerificationType.PhoneVerification);

                if (verification == null || verification.UserId != user.UserId || verification.IsUsed || verification.ExpireAt < DateTime.UtcNow)
                {
                    throw new UnauthorizedAccessException("Invalid or expired OTP.");
                }

                user.IsPhoneNumberVerified = true;
                user.UpdatedAt = DateTime.Now;
                var isUserUpdated = userRepo.Update(user);

                if (isUserUpdated)
                {
                    verification.IsUsed = true;
                    DataAccessFactory.VerificationData().Update(verification);
                    var auditLog = new AuditLogDTO
                    {
                        UserId = user.UserId,
                        Type = AuditLogType.PhoneVerified,
                        Details = $"User phone number verified for {user.PhoneNumber}",
                    };
                    AuditLogService.LogActivity(auditLog);
                }
                return isUserUpdated;
            }
            return false;
        }
    }
}
