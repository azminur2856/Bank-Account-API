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
        public static Task<bool> SendEmailVerificationLink(UserDTO user)
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

            var verificationLink = $"{ServiceFactory.ApiBaseUrl}/api/user/verifyaccount/{vToken}";

            ServiceFactory.EmailService.SendActivationEmail(user.Email, user.FullName, verificationLink);

            return Task.FromResult(true);
        }

        public static bool VerifyEmail(string token)
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
                    Type = AuditLogType.EmailVrified,
                    Details = $"User email verified for {user.Email}",
                };
                AuditLogService.LogActivity(auditLog);
            }

            ServiceFactory.EmailService.SendWelcomeEmail(user.Email, user.FullName, ServiceFactory.ApiBaseUrl);

            return isUserUpdated;
        }
    }
}
