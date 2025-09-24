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
    public class AuthService
    {
        public static Mapper GetMapper() {
            var config = new MapperConfiguration(cfg => {
                cfg.CreateMap<Token, TokenDTO>();
            });
            return new Mapper(config);
        }

        public static TokenDTO Authenticate(LoginDTO log)
        {
            var user = DataAccessFactory.UserFeaturesData().GetByEmail(log.Email);

            if (user == null)
            {
                throw new KeyNotFoundException("User not found with the provided email.");
            }

            if (user.IsEmailVerified == false && user.IsActive == false)
            {
                throw new UnauthorizedAccessException("Your email is not verified and your account is inactive. Please verify your email to complete registration.");
            }

            if(user.IsActive == false)
            {
                throw new UnauthorizedAccessException("Your account is inactive. Please contact support.");
            }

            bool isValid = BCrypt.Net.BCrypt.Verify(log.Password, user.PasswordHash);

            if (!isValid)
            {
                throw new UnauthorizedAccessException("Password does not match.");
            }

            var token = new Token() {
                TokenKey = Guid.NewGuid().ToString(),
                CreatedAt = DateTime.Now,
                ExpireAt = null,
                UserId = user.UserId
            };

            var tk = DataAccessFactory.TokenData().Create(token);

            if(tk != null) {
                var auditLog = new AuditLogDTO
                {
                    UserId = user.UserId,
                    Type = AuditLogType.UserLogin,
                    Details = $"User '{user.FullName}' (ID: {user.UserId}) logged in at {DateTime.Now:yyyy-MM-dd HH:mm:ss}.",
                };
                AuditLogService.LogActivity(auditLog);
            }

            return GetMapper().Map<TokenDTO>(tk);
        }

        public static bool IsTokenValid(string tokenKey)
        {
            var token = DataAccessFactory.TokenData().Get(tokenKey);
            if (token != null && token.ExpireAt == null)
            {
                return true;
            }
            return false;
        }

        public static bool IsAdmin(string tokenKey)
        {
            var token = DataAccessFactory.TokenData().Get(tokenKey);
            if (token != null && token.ExpireAt == null && token.User.Role == UserRole.Admin)
            {
                return true;
            }
            return false;
        }

        public static string GetUserRoleByToken(string tokenKey)
        {
            var token = DataAccessFactory.TokenData().Get(tokenKey);
            if (token != null && token.ExpireAt == null)
            {
                return token.User.Role.ToString(); ;
            }
            return null;
        }

        public static bool Logout(string tokenKey)
        {
            var token = DataAccessFactory.TokenData().Get(tokenKey);
            if (token != null && token.ExpireAt == null)
            {
                token.ExpireAt = DateTime.Now;
                var result = DataAccessFactory.TokenData().Update(token);
                if(result != null) {
                    var user = DataAccessFactory.UserData().Get(token.UserId);
                    if(user != null) {
                        var auditLog = new AuditLogDTO
                        {
                            UserId = user.UserId,
                            Type = AuditLogType.UserLogout,
                            Details = $"User '{user.FullName}' (ID: {user.UserId}) logged out at {DateTime.Now:yyyy-MM-dd HH:mm:ss}.",
                        };
                        AuditLogService.LogActivity(auditLog);
                    }
                }
                return result != null;
            }
            return false;
        }
    }
}
