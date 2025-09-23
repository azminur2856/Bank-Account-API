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
                var emailSend = await VerificationService.SendEmailVerificationLink(user);

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
    }
}
