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
                cfg.CreateMap<User, UserAddressesAccountsDTO>().ReverseMap();
                cfg.CreateMap<Address, AddressDTO>().ReverseMap();
                cfg.CreateMap<Account, AccountDTO>().ReverseMap();
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
                    DataAccessFactory.TokenFeaturesData().ExpireAllByUserId(user.UserId);
                    var auditLog = new AuditLogDTO
                    {
                        UserId = user.UserId,
                        Type = AuditLogType.PasswordChanged,
                        Details = $"User '{user.FullName}' (ID: {user.UserId}) changed their password at {DateTime.Now:yyyy-MM-dd HH:mm:ss}. All sessions expired.",
                    };
                    AuditLogService.LogActivity(auditLog);

                    ServiceFactory.EmailService.SendNotificationEmail(
                        user.Email,
                        user.FullName,
                        "Password Changed Successfully",
                        $"<p>Your password has been changed successfully. If you did not request this change, please contact our support team immediately.</p>"
                    );
                }
                return isUpdated;
            }
            return false;
        }

        public static bool ChangeUserRole(string adminTokenKey, RoleChangeDTO roleChange)
        {
            var adminToken = DataAccessFactory.TokenData().Get(adminTokenKey);
            if (adminToken == null || adminToken.ExpireAt != null) return false;

            var adminUser = DataAccessFactory.UserData().Get(adminToken.UserId);
            if (adminUser.Role != UserRole.Admin)
            {
                throw new UnauthorizedAccessException("Only administrators can change user roles.");
            }

            var userToUpdate = DataAccessFactory.UserFeaturesData().GetByEmail(roleChange.Email);
            if (userToUpdate == null)
            {
                throw new KeyNotFoundException("User not found with the provided email.");
            }

            if (userToUpdate.Role == UserRole.Admin)
            {
                throw new InvalidOperationException("Cannot change the role of another administrator.");
            }

            var (isValid, newRole, validNames) = Helper.ParseEnum<UserRole>(roleChange.NewRole);
            if (!isValid)
            {
                throw new ArgumentException($"Invalid role provided. Valid roles are: {string.Join(", ", validNames)}.");
            }

            if (newRole == UserRole.Admin)
            {
                throw new InvalidOperationException("Administrators cannot create new administrators now.");
            }

            var oldRole = userToUpdate.Role;

            if (oldRole == newRole)
            {
                throw new InvalidOperationException($"The user is already assigned the role '{newRole}'.");
            }

            userToUpdate.Role = newRole;
            userToUpdate.UpdatedAt = DateTime.Now;
            var isUpdated = DataAccessFactory.UserData().Update(userToUpdate);

            if (isUpdated)
            {
                string subject;
                string headline;
                string messageBody;
                AuditLogType auditLogType;

                if (newRole == UserRole.Employee)
                {
                    subject = "Appointment Letter: Your New Role at ART Bank PLC";
                    headline = "Congratulations!";
                    messageBody = $"<p>Dear {userToUpdate.FullName},</p><p>We are pleased to inform you that you have been appointed to the role of <strong>Employee</strong> at ART Bank PLC. We look forward to your contributions.</p><p>You can now access new features on your account dashboard.</p>";
                    auditLogType = AuditLogType.RoleChangedToEmployee;
                }
                else
                {
                    subject = "Important Update: Your Role at ART Bank PLC";
                    headline = "Update on Your Employment";
                    messageBody = $"<p>Dear {userToUpdate.FullName},</p><p>This letter is to inform you that your role at ART Bank PLC has been changed to <strong>Customer</strong>. Your access to employee-specific resources has been revoked.</p><p>We thank you for your service and look forward to continuing our relationship as a customer.</p>";
                    auditLogType = AuditLogType.RoleChangedToCustomer;
                }

                ServiceFactory.EmailService.SendCustomEmail(
                    userToUpdate.Email,
                    userToUpdate.FullName,
                    subject,
                    headline,
                    messageBody
                );

                var auditLog = new AuditLogDTO
                {
                    UserId = userToUpdate.UserId,
                    Type = auditLogType,
                    Details = $"Admin '{adminUser.FullName}' (ID: {adminUser.UserId}) changed user '{userToUpdate.FullName}' (ID: {userToUpdate.UserId}) role from '{oldRole}' to '{newRole}'.",
                };
                AuditLogService.LogActivity(auditLog);
            }
            return isUpdated;
        }

        public static bool ChangeUserStatus(string managerTokenKey, UserStatusChangeDTO statusChangeDTO)
        {
            var managerToken = DataAccessFactory.TokenData().Get(managerTokenKey);
            if (managerToken == null || managerToken.ExpireAt != null) return false;

            var managerUser = DataAccessFactory.UserData().Get(managerToken.UserId);
            var userToUpdate = DataAccessFactory.UserFeaturesData().GetByEmail(statusChangeDTO.Email);

            if (userToUpdate == null)
            {
                throw new KeyNotFoundException("User not found with the provided email.");
            }

            if (managerUser.Role == UserRole.Employee && userToUpdate.Role != UserRole.Customer)
            {
                throw new UnauthorizedAccessException("Employees can only change the status of customers.");
            }
            if (userToUpdate.Role == UserRole.Admin)
            {
                throw new InvalidOperationException("Cannot change the status of an administrator.");
            }

            if (userToUpdate.IsActive == statusChangeDTO.IsActive)
            {
                throw new InvalidOperationException($"The user is already {(statusChangeDTO.IsActive ? "active" : "inactive")}.");
            }

            userToUpdate.IsActive = statusChangeDTO.IsActive;
            userToUpdate.UpdatedAt = DateTime.Now;

            var isUpdated = DataAccessFactory.UserData().Update(userToUpdate);

            if (isUpdated)
            {
                if (statusChangeDTO.IsActive)
                {
                    ServiceFactory.EmailService.SendAccountActivatedEmail(userToUpdate.Email, userToUpdate.FullName);
                }
                else
                {
                    ServiceFactory.EmailService.SendAccountDeactivatedEmail(userToUpdate.Email, userToUpdate.FullName);
                }

                var auditLogType = statusChangeDTO.IsActive
                    ? AuditLogType.UserActivated
                    : AuditLogType.UserDeactivated;

                var auditLog = new AuditLogDTO
                {
                    UserId = userToUpdate.UserId,
                    Type = auditLogType,
                    Details = $"{managerUser.Role} '{managerUser.FullName}' (ID: {managerUser.UserId}) changed user '{userToUpdate.FullName}' (ID: {userToUpdate.UserId}) status to '{statusChangeDTO.IsActive}'.",
                };
                AuditLogService.LogActivity(auditLog);
            }
            return isUpdated;
        }

        public static UserAddressesAccountsDTO GetProfileDetails(string tokenKey)
        {
            var token = DataAccessFactory.TokenData().Get(tokenKey);
            if (token == null || token.ExpireAt != null)
            {
                throw new UnauthorizedAccessException("Invalid or expired token.");
            }

            var user = DataAccessFactory.UserData().Get(token.UserId);
            if (user == null)
            {
                throw new KeyNotFoundException("User not found.");
            }
            var userProfile = GetMapper().Map<UserAddressesAccountsDTO>(user);
            
            userProfile.Addresses = GetMapper().Map<List<AddressDTO>>(DataAccessFactory.AddressFeaturesData().GetByUserId(user.UserId));
            userProfile.Accounts = GetMapper().Map<List<AccountDTO>>(DataAccessFactory.AccountFeaturesData().GetByUserId(user.UserId));

            userProfile.PasswordHash = null;

            return userProfile;
        }
    }
}
