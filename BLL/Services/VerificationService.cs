using AutoMapper;
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

        public static Mapper GetMapper()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<User, UserDTO>().ReverseMap();
            });
            return new Mapper(config);
        }

        public static Task<bool> SendEmailVerificationLink(UserDTO user, bool isRegistration = false)
        {
            var vToken = Helper.GenerateToken();

            var expireAt = isRegistration ? DateTime.UtcNow.AddHours(24) : DateTime.UtcNow.AddHours(1);

            var verification = new Verification
            {
                Code = vToken,
                Type = VerificationType.EmailVerification,
                IsUsed = false,
                CreatedAt = DateTime.UtcNow,
                ExpireAt = expireAt,
                UserId = user.UserId
            };

            var isCreated = DataAccessFactory.VerificationData().Create(verification);

            if (!isCreated) return Task.FromResult(false);

            string verificationLink;

            if (isRegistration)
            {
                verificationLink = $"{ServiceFactory.ApiBaseUrl}/api/user/verifyaccount/{vToken}";
                ServiceFactory.EmailService.SendActivationEmail(user.Email, user.FullName, verificationLink);
            }
            else
            {
                verificationLink = $"{ServiceFactory.ApiBaseUrl}/api/user/verifyemail/{vToken}";
                ServiceFactory.EmailService.SendVerifyEmail(user.Email, user.FullName, verificationLink);
            }

            var auditLog = new AuditLogDTO
            {
                UserId = user.UserId,
                Type = AuditLogType.EmailVerificationLinkSent,
                Details = isRegistration
                    ? $"Account activation email sent to {user.Email} with token {vToken}"
                    : $"Email verification link sent to {user.Email} with token {vToken}",
            };

            AuditLogService.LogActivity(auditLog);

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

        public static async Task<bool> SendEmailVerificationToken(string tokenKey) {
            var token = DataAccessFactory.TokenData().Get(tokenKey);
            if (token != null && token.ExpireAt == null)
            {
                var userRepo = DataAccessFactory.UserData();
                var user = userRepo.Get(token.UserId);

                if (user == null)
                {
                    throw new KeyNotFoundException("User not found.");
                }

                if (user.IsEmailVerified)
                {
                    throw new UnauthorizedAccessException("Email is already verified.");
                }

                var emailSend = await SendEmailVerificationLink((GetMapper().Map<UserDTO>(user)), false);

                if (!emailSend)
                {
                    return false;
                }
                return emailSend;
            }
            return false;
        }

        public static bool VerifyEmail(string token)
        {
            var verification = DataAccessFactory.VerificationFeaturesData().GetByCodeAndType(token, VerificationType.EmailVerification);

            if (verification == null) return false;

            var userRepo = DataAccessFactory.UserData();
            var user = userRepo.Get(verification.UserId);

            if (user == null) return false;

            user.IsEmailVerified = true;
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
                    Details = $"User email verified for {user.Email}",
                };
                AuditLogService.LogActivity(auditLog);
            }

            ServiceFactory.EmailService.SendCustomEmail(
                user.Email,
                user.FullName,
                "Email Verified",
                "Verification Successful",
                $"<p>Your email has been successfully verified.</p><p>Thank you!</p>"
            );
            return isUserUpdated;
        }

        public static async Task<bool> SendPasswordResetOTP(PasswordResetRequestDTO prrd) 
        {
            var user = DataAccessFactory.UserFeaturesData().GetByEmail(prrd.Email);
            if(user == null)
            {
                throw new KeyNotFoundException("User not found with the provided email.");
            }

            var type = string.IsNullOrWhiteSpace(prrd.Type) ? "Email" : prrd.Type.Trim();
            type = type.ToLowerInvariant();

            if (type == "email")
            {
                if(!user.IsEmailVerified)
                {
                    throw new ArgumentException("Email is not verified.");
                }
            }
            else if(type == "sms")
            {
                if(!user.IsPhoneNumberVerified)
                {
                    throw new ArgumentException("Phone number is not verified.");
                }
            }
            else
            {
                throw new ArgumentException("Invalid type. Must be 'Email', 'Sms'.");
            }

            var otp = Helper.GenerateOtp();

            var verification = new Verification
            {
                Code = otp,
                Type = VerificationType.PasswordReset,
                IsUsed = false,
                CreatedAt = DateTime.UtcNow,
                ExpireAt = DateTime.UtcNow.AddMinutes(5),
                UserId = user.UserId
            };

            var isCreated = DataAccessFactory.VerificationData().Create(verification);
            
            if (!isCreated) return false;

            if(type == "email")
            {
                ServiceFactory.EmailService.SendPasswordResetOtpEmail(user.Email, user.FullName, otp);
                var auditLog = new AuditLogDTO
                {
                    UserId = user.UserId,
                    Type = AuditLogType.PasswordResetEmailOtpRequested,
                    Details = $"Password reset OTP sent to email for user ID {user.UserId}."
                };
                AuditLogService.LogActivity(auditLog);
            }
            else if (type == "sms")
            {
                await ServiceFactory.SmsService.SendSMSAsync(user.PhoneNumber, $"Your password reset OTP is {otp}. It is valid for 5 minutes.");
                var auditLog = new AuditLogDTO
                {
                    UserId = user.UserId,
                    Type = AuditLogType.PasswordResetSmsOtpRequested,
                    Details = $"Password reset OTP sent to phone number for user ID {user.UserId}."
                };
                AuditLogService.LogActivity(auditLog);
            }
            return isCreated;
        }

        public static bool ResetPassword(PasswordResetDTO prd)
        {
            var verification = DataAccessFactory.VerificationFeaturesData().GetByCodeAndType(prd.ResetOtp, VerificationType.PasswordReset);

            if (verification == null) return false;

            var user = DataAccessFactory.UserFeaturesData().GetByEmail(prd.Email);

            if (user == null) return false;

            if(verification.UserId != user.UserId || verification.IsUsed || verification.ExpireAt < DateTime.UtcNow)
            {
                return false;
            }

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(prd.NewPassword);
            user.UpdatedAt = DateTime.Now;

            var isUserUpdated = DataAccessFactory.UserData().Update(user);

            if (isUserUpdated)
            {
                verification.IsUsed = true;
                DataAccessFactory.VerificationData().Update(verification);

                var auditLog = new AuditLogDTO()
                {
                    UserId = user.UserId,
                    Type = AuditLogType.PasswordResetCompleted,
                    Details = $"User '{user.FullName}' (ID: {user.UserId}) successfully reset their password."
                };
                AuditLogService.LogActivity(auditLog);

                ServiceFactory.EmailService.SendNotificationEmail(
                        user.Email,
                        user.FullName,
                        "Password Reset Alart",
                        $"<p>Your password has been successfully reset. If you did not make this change, please contact support immediately.</p>"
                );
            }
            return isUserUpdated;
        }
    }
}
