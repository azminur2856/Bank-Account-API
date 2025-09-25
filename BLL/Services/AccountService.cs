using AutoMapper;
using BLL.DTOs;
using BLL.Utility;
using DAL;
using DAL.EF.Tables;
using DAL.Enums;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Services
{
    public class AccountService
    {
        public static Mapper GetMapper() {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Account, AccountDTO>().ReverseMap();
            });
            return new Mapper(config);
        }

        public static AccountDTO Create(string tokenKey, AccountCreateDTO accountDto)
        {
            var token = DataAccessFactory.TokenData().Get(tokenKey);
            if (token == null || token.ExpireAt != null)
            {
                throw new UnauthorizedAccessException("Invalid or expired token.");
            }
            var performedByUser = DataAccessFactory.UserData().Get(token.UserId);

            User userToCreateAccountFor;
            bool isActive;
            AuditLogType auditLogType;

            if (performedByUser.Role == UserRole.Customer)
            {
                if(accountDto == null)
                {
                    throw new InvalidOperationException($"Account data is required.1. AccountType: Savings or Current 2. Email");
                }
                if (!string.IsNullOrEmpty(accountDto.Email) && !accountDto.Email.Equals(performedByUser.Email, StringComparison.OrdinalIgnoreCase))
                {
                    throw new UnauthorizedAccessException("A customer can only create an account for themselves.");
                }

                userToCreateAccountFor = performedByUser;

                var userAddresses = DataAccessFactory.AddressFeaturesData().GetByUserId(userToCreateAccountFor.UserId);
                bool hasPresentAddress = userAddresses.Any(a => a.Type == AddressType.Present);
                bool hasPermanentAddress = userAddresses.Any(a => a.Type == AddressType.Permanent);
                bool allAddressesVerified = userAddresses.All(a => a.IsVerified);

                if (!userToCreateAccountFor.IsEmailVerified || !userToCreateAccountFor.IsPhoneNumberVerified || !hasPresentAddress || !hasPermanentAddress || !allAddressesVerified)
                {
                    throw new InvalidOperationException("Cannot create an account. The user's email, phone, or addresses are not verified.");
                }

                isActive = false;
                auditLogType = AuditLogType.AccountCreatedByCustomer;
            }
            else if (performedByUser.Role == UserRole.Employee || performedByUser.Role == UserRole.Admin)
            {
                if (accountDto == null)
                {
                    throw new InvalidOperationException($"Account data is required. 1. AccountType: Master or Savings or Current 2. Email");
                }
                if (string.IsNullOrEmpty(accountDto.Email))
                {
                    throw new InvalidOperationException("Email is required for user account creation.");
                }
                userToCreateAccountFor = DataAccessFactory.UserFeaturesData().GetByEmail(accountDto.Email);
                if (userToCreateAccountFor == null)
                {
                    throw new KeyNotFoundException("User not found.");
                }

                if (userToCreateAccountFor.Role == UserRole.Admin && performedByUser.Role == UserRole.Employee)
                {
                    throw new UnauthorizedAccessException("An employee cannot create an account for an administrator.");
                }

                var userAddresses = DataAccessFactory.AddressFeaturesData().GetByUserId(userToCreateAccountFor.UserId);
                bool hasPresentAddress = userAddresses.Any(a => a.Type == AddressType.Present);
                bool hasPermanentAddress = userAddresses.Any(a => a.Type == AddressType.Permanent);
                bool allAddressesVerified = userAddresses.All(a => a.IsVerified);

                if (!userToCreateAccountFor.IsEmailVerified || !userToCreateAccountFor.IsPhoneNumberVerified || !hasPresentAddress || !hasPermanentAddress || !allAddressesVerified)
                {
                    throw new InvalidOperationException("Cannot create an active account. The user's email, phone, or addresses are not verified.");
                }

                isActive = true;
                auditLogType = AuditLogType.AccountCreatedByEmployee;
            }
            else
            {
                throw new UnauthorizedAccessException("You do not have permission to create an account.");
            }

            var (isValid, accountType, validNames) = Helper.ParseEnum<AccountType>(accountDto.AccountType);
            if (!isValid)
            {
                throw new ArgumentException($"Invalid account type '{accountDto.AccountType}'. Allowed values are: {string.Join(", ", validNames)}.");
            }
            if (userToCreateAccountFor.Role == UserRole.Admin && accountType != AccountType.Master)
            {
                throw new InvalidOperationException("Administrators can only have a Master account.");
            }
            if ((userToCreateAccountFor.Role == UserRole.Employee || userToCreateAccountFor.Role == UserRole.Customer) && accountType == AccountType.Master)
            {
                throw new InvalidOperationException("Employees and Customers cannot have a Master account.");
            }

            var existingAccounts = DataAccessFactory.AccountFeaturesData().GetByUserId(userToCreateAccountFor.UserId);
            if (existingAccounts.Any(a => a.Type == accountType))
            {
                throw new InvalidOperationException($"The user already has a '{accountType}' account.");
            }

            var lastAccount = DataAccessFactory.AccountFeaturesData().GetLastAccount();
            var newAccountNumber = Helper.GenerateNewAccountNumber(lastAccount);

            var newAccount = new Account
            {
                UserId = userToCreateAccountFor.UserId,
                AccountNumber = newAccountNumber,
                Type = accountType,
                Balance = 0,
                IsActive = isActive,
                CreatedBy = performedByUser.UserId,
                CreatedAt = DateTime.Now
            };

            var createdAccount = DataAccessFactory.AccountData().Create(newAccount);

            string balanceFormatted = "TK " + createdAccount.Balance.ToString("N2", CultureInfo.InvariantCulture);

            if (createdAccount != null)
            {
                ServiceFactory.EmailService.SendNewAccountDetailsEmail(
                    userToCreateAccountFor.Email,
                    userToCreateAccountFor.FullName,
                    createdAccount.AccountNumber,
                    createdAccount.Type.ToString(),
                    balanceFormatted,
                    createdAccount.CreatedAt.ToString("g")
                );

                var auditLog = new AuditLogDTO
                {
                    UserId = performedByUser.UserId,
                    Type = auditLogType,
                    Details = $"{performedByUser.Role} '{performedByUser.FullName}' (ID: {performedByUser.UserId}) created a new '{accountType}' account for user '{userToCreateAccountFor.FullName}' (ID: {userToCreateAccountFor.UserId})."
                };
                AuditLogService.LogActivity(auditLog);

                if(isActive)
                {
                    if (performedByUser.Role == UserRole.Admin)
                    {
                        var activationLog = new AuditLogDTO
                        {
                            UserId = performedByUser.UserId,
                            Type = AuditLogType.AccountActivatedByAdmin,
                            Details = $"Account '{createdAccount.AccountNumber}' (ID: {createdAccount.AccountId}) activated by Admin '{performedByUser.FullName}' (ID: {performedByUser.UserId})."
                        };
                        AuditLogService.LogActivity(activationLog);
                    }
                    else if (performedByUser.Role == UserRole.Employee)
                    {
                        var activationLog = new AuditLogDTO
                        {
                            UserId = performedByUser.UserId,
                            Type = AuditLogType.AccountActivatedByEmployee,
                            Details = $"Account '{createdAccount.AccountNumber}' (ID: {createdAccount.AccountId}) activated by Employee '{performedByUser.FullName}' (ID: {performedByUser.UserId})."
                        };
                        AuditLogService.LogActivity(activationLog);
                    }
                }

                return GetMapper().Map<AccountDTO>(createdAccount);
            }

            return null;
        }

        public static bool ActivateAccount(string tokenKey, string accountNumber)
        {
            var token = DataAccessFactory.TokenData().Get(tokenKey);
            if (token == null || token.ExpireAt != null)
            {
                throw new UnauthorizedAccessException("Invalid or expired token.");
            }

            var performingUser = DataAccessFactory.UserData().Get(token.UserId);
            if (performingUser.Role != UserRole.Employee)
            {
                throw new UnauthorizedAccessException("Only an employee can activate customer accounts.");
            }

            var accountToActivate = DataAccessFactory.AccountData().Get(accountNumber);
            if (accountToActivate == null)
            {
                throw new KeyNotFoundException("Account not found.");
            }

            if (accountToActivate.IsActive)
            {
                throw new InvalidOperationException("Account is already active.");
            }

            var owner = DataAccessFactory.UserData().Get(accountToActivate.UserId);
            var ownerAddresses = DataAccessFactory.AddressFeaturesData().GetByUserId(owner.UserId);
            bool allAddressesVerified = ownerAddresses.All(a => a.IsVerified);

            if (!owner.IsEmailVerified || !owner.IsPhoneNumberVerified || !allAddressesVerified)
            {
                throw new InvalidOperationException("Cannot activate account. User's email, phone, or addresses are not verified.");
            }

            accountToActivate.IsActive = true;
            accountToActivate.UpdatedAt = DateTime.Now;
            var isUpdated = DataAccessFactory.AccountData().Update(accountToActivate);

            if (isUpdated != null)
            {
                ServiceFactory.EmailService.SendAccountActivatedEmail(owner.Email, owner.FullName);

                var auditLog = new AuditLogDTO
                {
                    UserId = performingUser.UserId,
                    Type = AuditLogType.AccountActivatedByEmployee,
                    Details = $"Account '{accountToActivate.AccountNumber}' (ID: {accountToActivate.AccountId}) activated by Employee '{performingUser.FullName}' (ID: {performingUser.UserId})."
                };
                AuditLogService.LogActivity(auditLog);
            }

            return isUpdated != null;
        }

        public static List<AccountDTO> GetAccounts(string tokenKey)
        {
            var token = DataAccessFactory.TokenData().Get(tokenKey);
            if (token == null || token.ExpireAt != null) return null;
            
            var accounts = DataAccessFactory.AccountData().Get();
           
            return GetMapper().Map<List<AccountDTO>>(accounts);
        }

        public static List<AccountDTO> GetUserAccounts(string tokenKey)
        {
            var token = DataAccessFactory.TokenData().Get(tokenKey);
            if (token == null || token.ExpireAt != null) return null;
            return GetMapper().Map<List<AccountDTO>>(DataAccessFactory.AccountFeaturesData().GetByUserId(token.UserId));
        }

        public static AccountDTO Get(string AccountNumber)
        {
            return GetMapper().Map<AccountDTO>(DataAccessFactory.AccountData().Get(AccountNumber));
        }

    }
}
