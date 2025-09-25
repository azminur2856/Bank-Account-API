using AutoMapper;
using BLL.DTOs;
using DAL;
using DAL.EF.Tables;
using DAL.Enums;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BLL.Utility;

namespace BLL.Services
{
    public class TransactionService
    {
        private const decimal TRANSFER_FEE = 10;
        private static readonly Dictionary<AccountType, decimal> MINIMUM_BALANCES = new Dictionary<AccountType, decimal>
        {
            { AccountType.Master, 10000m },
            { AccountType.Savings, 500m },
            { AccountType.Current, 1000m }
        };

        public static Mapper GetMapper()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Transaction, TransactionDTO>().ReverseMap();
            });
            return new Mapper(config);
        }

        private static bool CheckMinimumBalance(Account account, decimal amount, decimal fees)
        {
            return account.Balance - amount - fees >= MINIMUM_BALANCES[account.Type];
        }

        private static void CreateAndLogTransaction(Account sourceAccount, Account destinationAccount, decimal amount, decimal fees, TransactionType type, User performedByUser)
        {
            var sourceType = sourceAccount != null ? (TransactionType?)type : null;
            var destinationType = destinationAccount != null ? (TransactionType?)type : null;

            if (type == TransactionType.SystemCredit)
            {
                sourceType = null;
                destinationType = TransactionType.SystemCredit;
            }
            else if (type == TransactionType.SystemDebit)
            {
                sourceType = TransactionType.SystemDebit;
                destinationType = null;
            }
            else if (type == TransactionType.Deposit)
            {
                sourceType = TransactionType.SystemDebit;
                destinationType = TransactionType.Deposit;
            }
            else if (type == TransactionType.Withdrawal)
            {
                sourceType = TransactionType.Withdrawal;
                destinationType = TransactionType.SystemCredit;
            }
            else if (type == TransactionType.Transfer)
            {
                sourceType = TransactionType.Transfer;
                destinationType = TransactionType.Deposit;
            }

            var transaction = new Transaction
            {
                SourceAccountId = sourceAccount?.AccountId,
                DestinationAccountId = destinationAccount?.AccountId,
                Amount = amount,
                Fees = fees,
                Type = type,
                SourceType = sourceType,
                DestinationType = destinationType,
                PerformedBy = performedByUser.UserId,
                CreatedAt = DateTime.Now
            };
            DataAccessFactory.TransactionData().Create(transaction);

            var auditLog = new AuditLogDTO
            {
                UserId = performedByUser.UserId,
                Type = (AuditLogType)Enum.Parse(typeof(AuditLogType), type.ToString()),
                Details = $"Transaction performed by user '{performedByUser.FullName}' (ID: {performedByUser.UserId}). Type: {type}, Amount: {amount}."
            };
            AuditLogService.LogActivity(auditLog);

            if (fees > 0)
            {
                var feeTransaction = new Transaction
                {
                    SourceAccountId = sourceAccount?.AccountId,
                    DestinationAccountId = DataAccessFactory.AccountData().Get(ServiceFactory.BankMasterAccountNumber).AccountId,
                    Amount = fees,
                    Type = TransactionType.SystemCredit,
                    SourceType = TransactionType.Transfer,
                    DestinationType = TransactionType.SystemCredit,
                    PerformedBy = performedByUser.UserId,
                    CreatedAt = DateTime.Now
                };
                DataAccessFactory.TransactionData().Create(feeTransaction);

                var feeLog = new AuditLogDTO
                {
                    UserId = performedByUser.UserId,
                    Type = AuditLogType.TransferFee,
                    Details = $"Transfer fee of {fees} debited from account '{sourceAccount.AccountNumber}' and credited to bank master account."
                };
                AuditLogService.LogActivity(feeLog);
            }

            string amountFormatted = "TK " + amount.ToString("N2", CultureInfo.InvariantCulture);

            if (sourceAccount != null)
            {
                ServiceFactory.EmailService.SendTransactionNotificationEmail(
                sourceAccount.User.Email,
                sourceAccount.User.FullName,
                sourceType.ToString(),
                amountFormatted,
                DateTime.Now.ToString("g"),
                transaction.TransactionId.ToString()
                );
            }

            if(destinationAccount != null)
            {
                ServiceFactory.EmailService.SendTransactionNotificationEmail(
                destinationAccount.User.Email,
                destinationAccount.User.FullName,
                destinationType.ToString(),
                amountFormatted,
                DateTime.Now.ToString("g"),
                transaction.TransactionId.ToString()
                );
            }
        }

        public static bool CreateSystemCredit(string tokenKey, decimal amount)
        {
            var token = DataAccessFactory.TokenData().Get(tokenKey);
            if (token == null || token.ExpireAt != null) return false;

            var performingUser = DataAccessFactory.UserData().Get(token.UserId);
            if (performingUser.Role != UserRole.Admin)
            {
                throw new UnauthorizedAccessException("Only administrators can perform system credits.");
            }

            var account = DataAccessFactory.AccountData().Get(ServiceFactory.BankMasterAccountNumber);
            if (account == null)
            {
                throw new KeyNotFoundException("Account not found.");
            }

            account.Balance += amount;
            var isUpdated = DataAccessFactory.AccountData().Update(account);

            if (isUpdated != null)
            {
                CreateAndLogTransaction(null, account, amount, 0, TransactionType.SystemCredit, performingUser);
            }
            return isUpdated != null;
        }

        public static bool CreateSystemDebit(string tokenKey, decimal amount)
        {
            var token = DataAccessFactory.TokenData().Get(tokenKey);
            if (token == null || token.ExpireAt != null) return false;

            var performingUser = DataAccessFactory.UserData().Get(token.UserId);
            if (performingUser.Role != UserRole.Admin)
            {
                throw new UnauthorizedAccessException("Only administrators can perform system debits.");
            }

            var account = DataAccessFactory.AccountData().Get(ServiceFactory.BankMasterAccountNumber);
            if (account == null)
            {
                throw new KeyNotFoundException("Account not found.");
            }
            if (!CheckMinimumBalance(account, amount, 0))
            {
                throw new InvalidOperationException("Insufficient balance to perform system debit.");
            }

            account.Balance -= amount;
            var isUpdated = DataAccessFactory.AccountData().Update(account);

            if (isUpdated != null)
            {
                CreateAndLogTransaction(account, null, amount, 0, TransactionType.SystemDebit, performingUser);
            }
            return isUpdated != null;
        }

        public static bool CreateDeposit(string tokenKey, DepositDTO depositDTO)
        {
            var token = DataAccessFactory.TokenData().Get(tokenKey);
            if (token == null || token.ExpireAt != null) return false;

            var user = DataAccessFactory.UserData().Get(token.UserId);
            if (user.Role != UserRole.Employee)
            {
                throw new UnauthorizedAccessException("Only employees can deposit into customer accounts.");
            }

            var accountRepo = DataAccessFactory.AccountData();
            var destinationAccount = accountRepo.Get(depositDTO.DestinationAccountNumber);
            var bankMasterAccount = accountRepo.Get(ServiceFactory.BankMasterAccountNumber);

            if (destinationAccount == null)
            {
                throw new KeyNotFoundException("Destination account not found.");
            }

            if (destinationAccount.UserId == user.UserId || destinationAccount.User.Role == UserRole.Admin)
            {
                throw new InvalidOperationException("Cannot deposit into the employee's own account or admin accounts.");
            }

            if (bankMasterAccount == null)
            {
                throw new KeyNotFoundException("Bank master account not found.");
            }
            if (!destinationAccount.IsActive || !bankMasterAccount.IsActive)
            {
                throw new InvalidOperationException("Source or destination account is not active.");
            }
            if (!CheckMinimumBalance(bankMasterAccount, depositDTO.Amount, 0))
            {
                throw new InvalidOperationException("Insufficient balance in the bank's master account.");
            }

            bankMasterAccount.Balance -= depositDTO.Amount;
            destinationAccount.Balance += depositDTO.Amount;

            var isUpdated = accountRepo.Update(bankMasterAccount) != null && accountRepo.Update(destinationAccount) != null;

            if (isUpdated)
            {
                CreateAndLogTransaction(bankMasterAccount, destinationAccount, depositDTO.Amount, 0, TransactionType.Deposit, user);
                return true;
            }
            return false;
        }

        public static bool CreateWithdrawal(string tokenKey, WithdrawalDTO withdrawalDTO)
        {
            var token = DataAccessFactory.TokenData().Get(tokenKey);
            if (token == null || token.ExpireAt != null) return false;

            var user = DataAccessFactory.UserData().Get(token.UserId);
            var accountRepo = DataAccessFactory.AccountData();
            var sourceAccount = accountRepo.Get(withdrawalDTO.SourceAccountNumber);
            var bankMasterAccount = accountRepo.Get(ServiceFactory.BankMasterAccountNumber);

            if (sourceAccount == null || sourceAccount.UserId != user.UserId)
            {
                throw new KeyNotFoundException("Source account not found or does not belong to the user.");
            }
            if (bankMasterAccount == null)
            {
                throw new KeyNotFoundException("Bank master account not found.");
            }
            if (!sourceAccount.IsActive || !bankMasterAccount.IsActive)
            {
                throw new InvalidOperationException("Source or destination account is not active.");
            }
            if (!CheckMinimumBalance(sourceAccount, withdrawalDTO.Amount, 0))
            {
                throw new InvalidOperationException("Insufficient balance in the source account.");
            }

            sourceAccount.Balance -= withdrawalDTO.Amount;
            bankMasterAccount.Balance += withdrawalDTO.Amount;

            var isUpdated = accountRepo.Update(sourceAccount) != null && accountRepo.Update(bankMasterAccount) != null;

            if (isUpdated)
            {
                CreateAndLogTransaction(sourceAccount, bankMasterAccount, withdrawalDTO.Amount, 0, TransactionType.Withdrawal, user);
                return true;
            }
            return false;
        }

        public static bool CreateTransfer(string tokenKey, TransferDTO transferDTO)
        {
            var token = DataAccessFactory.TokenData().Get(tokenKey);
            if (token == null || token.ExpireAt != null) return false;

            var user = DataAccessFactory.UserData().Get(token.UserId);
            var accountRepo = DataAccessFactory.AccountData();
            var sourceAccount = accountRepo.Get(transferDTO.SourceAccountNumber);
            var destinationAccount = accountRepo.Get(transferDTO.DestinationAccountNumber);
            var bankMasterAccount = accountRepo.Get(ServiceFactory.BankMasterAccountNumber);

            if (sourceAccount == null || sourceAccount.UserId != user.UserId)
            {
                throw new KeyNotFoundException("Source account not found or does not belong to the user.");
            }
            if (destinationAccount == null)
            {
                throw new KeyNotFoundException("Destination account not found.");
            }
            if (sourceAccount.AccountNumber.Equals(destinationAccount.AccountNumber))
            {
                throw new InvalidOperationException("Cannot transfer funds to the same account.");
            }
            if (!sourceAccount.IsActive || !destinationAccount.IsActive)
            {
                throw new InvalidOperationException("Source or destination account is not active.");
            }
            if (!CheckMinimumBalance(sourceAccount, transferDTO.Amount, TRANSFER_FEE))
            {
                throw new InvalidOperationException($"Insufficient balance. A transfer of {transferDTO.Amount} with a fee of {TRANSFER_FEE} requires a minimum balance of {MINIMUM_BALANCES[sourceAccount.Type]} after the transaction.");
            }

            sourceAccount.Balance -= (transferDTO.Amount + TRANSFER_FEE);
            destinationAccount.Balance += transferDTO.Amount;
            bankMasterAccount.Balance += TRANSFER_FEE;

            var isUpdated = accountRepo.Update(sourceAccount) != null && accountRepo.Update(destinationAccount) != null && accountRepo.Update(bankMasterAccount) != null;

            if (isUpdated)
            {
                CreateAndLogTransaction(sourceAccount, destinationAccount, transferDTO.Amount, TRANSFER_FEE, TransactionType.Transfer, user);
                return true;
            }
            return false;
        }
    }
}