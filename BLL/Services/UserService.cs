using AutoMapper;
using BCrypt.Net;
using BLL.DTOs;
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
            var existingUser = DataAccessFactory.UserFeaturesData().GetByEmail(user.Email);

            if (existingUser != null)
            {
                return false;
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
