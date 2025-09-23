using AutoMapper;
using BCrypt.Net;
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
    public class UserService
    {
        public static Mapper GetMapper()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<User, UserDTO>().ReverseMap();
            });
            return new Mapper(config);
        }

        public static async Task<bool> Create(UserDTO user)
        {
            var existingUserEmail = DataAccessFactory.UserFeaturesData().GetByEmail(user.Email);

            if (existingUserEmail != null)
            {
                throw new InvalidOperationException("A user with this email already exists.");
            }

            if (!Helper.DomainHasMxRecord(user.Email))
            {
                throw new InvalidOperationException("The email domain is invalid or does not have a valid MX record.");
            }

            var existingUserPhone = DataAccessFactory.UserFeaturesData().GetByPhone(user.PhoneNumber);

            if (existingUserPhone != null)
            {
                throw new InvalidOperationException("A user with this phone number already exists.");
            }

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(user.PasswordHash);

            user.IsEmailVerified = false;
            user.IsPhoneNumberVerified = false;
            user.Role = UserRole.Customer;
            user.IsActive = false;
            user.CreatedAt = DateTime.Now;

            var result = DataAccessFactory.UserData().Create(GetMapper().Map<User>(user));
            
            if (result)
            {
                var createdUser = DataAccessFactory.UserFeaturesData().GetByEmail(user.Email);
                user.UserId = createdUser.UserId;
                var emailSend = await VerificationService.SendEmailVerificationLink(user, true);

                var auditLog = new AuditLogDTO
                {
                    UserId = user.UserId,
                    Type = AuditLogType.UserCreated,
                    Details = $"New user created with email {user.Email} and an email verification link has been sent.",
                };
                AuditLogService.LogActivity(auditLog);

                return emailSend;
            }
            return false;
        }

        public static bool ChangePassword(string tokenKey, PasswordChangeDTO cp)
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

                if (!BCrypt.Net.BCrypt.Verify(cp.OldPassword, user.PasswordHash)) 
                {
                    throw new UnauthorizedAccessException("Old password does not match.");
                }

                user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(cp.NewPassword);
                user.UpdatedAt = DateTime.Now;

                var isUpdated = userRepo.Update(user);

                if(isUpdated)
                {
                    var auditLog = new AuditLogDTO
                    {
                        UserId = user.UserId,
                        Type = AuditLogType.PasswordChanged,
                        Details = $"User '{user.FullName}' (ID: {user.UserId}) changed their password at {DateTime.Now:yyyy-MM-dd HH:mm:ss}.",
                    };
                    AuditLogService.LogActivity(auditLog);
                    // this just logout the current session
                    AuthService.Logout(tokenKey);
                    // TODO: Need to implement forced logout from all other sessions
                }
                return isUpdated;
            }
            return false;
        }
    }
}
