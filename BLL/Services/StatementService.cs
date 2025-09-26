using AutoMapper;
using BLL.DTOs;
using DAL;
using DAL.EF.Tables;
using DAL.Enums;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BLL.Services
{
    public class StatementService
    {       
        public static Mapper GetMapper()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Transaction, StatementTransactionDTO>()
                    .ForMember(
                        dest => dest.Date,
                        opt => opt.MapFrom(src => src.CreatedAt.ToString("yyyy-MM-dd"))
                    )
                    .ForMember(
                        dest => dest.Type,
                        opt => opt.MapFrom(src => src.Type.ToString("G"))
                    )
                    .ForMember(
                        dest => dest.Details,
                        opt => opt.MapFrom(src => GetTransactionDetails(src))
                    )
                    .ForMember(dest => dest.Debit, opt => opt.Ignore())
                    .ForMember(dest => dest.Credit, opt => opt.Ignore());
            });
            return new Mapper(config);
        }

        private static string GetTransactionDetails(Transaction transaction)
        {
            return transaction.Type.ToString("G");
        }

        public static StatementDTO GetAccountStatementData(string tokenKey, StatementRequestDTO request)
        {
            var token = DataAccessFactory.TokenData().Get(tokenKey);

            if (request == null)
            {
                throw new ArgumentNullException("Account number, Start date and end date requried.");
            }

            var accountRepo = DataAccessFactory.AccountData();
            var account = accountRepo.Get(request.AccountNumber);
            if (account == null)
            {
                throw new KeyNotFoundException("Account not found.");
            }
            if (token.User.Role != UserRole.Employee && token.User.Role != UserRole.Admin && account.UserId != token.UserId)
            {
                throw new UnauthorizedAccessException("You are not authorized to access this account statement.");
            }

            int statementAccountId = account.AccountId;

            var transactions = DataAccessFactory.TransactionFeaturesData().GetByAccountAndDateRange(statementAccountId, request.StartDate, request.EndDate);

            var processedTransactions = new List<StatementTransactionDTO>();

            foreach (var transaction in transactions.OrderBy(t => t.CreatedAt))
            {
                var dto = GetMapper().Map<StatementTransactionDTO>(transaction);
                dto.Debit = 0m;
                dto.Credit = 0m;

                if (transaction.SourceAccountId == statementAccountId)
                {
                    //decimal totalDebitAmount = transaction.Amount + (transaction.Fees ?? 0m);

                    if (transaction.Type == TransactionType.Transfer)
                    {
                        var destAccount = DataAccessFactory.AccountFeaturesData().GetById(transaction.DestinationAccountId.Value);
                        dto.Details = $"Transfer to Account: {destAccount?.AccountNumber}";
                    }
                    dto.Debit = transaction.Amount; //totalDebitAmount;
                }
                else if (transaction.DestinationAccountId == statementAccountId)
                {
                    if (transaction.Type == TransactionType.Transfer)
                    {
                        var srcAccount = DataAccessFactory.AccountFeaturesData().GetById(transaction.SourceAccountId.Value);
                        dto.Details = $"Transfer from Account: {srcAccount?.AccountNumber}";
                    }
                    dto.Credit = transaction.Amount;
                }

                processedTransactions.Add(dto);
            }

            var user = DataAccessFactory.UserData().Get(account.UserId);
            var address = DataAccessFactory.AddressFeaturesData().GetByUserIdAndType(user.UserId, AddressType.Permanent);

            decimal endingBalance = account.Balance;
            decimal totalDebits = processedTransactions.Sum(t => t.Debit);
            decimal totalCredits = processedTransactions.Sum(t => t.Credit);
            decimal startingBalance = endingBalance - totalCredits + totalDebits;

            var statement = new StatementDTO
            {
                FullName = user.FullName,
                Address = $"{address?.StreetAddress}, {address?.City}, {address?.State}, {address?.Country}-{address?.PostalCode}",
                AccountNumber = account.AccountNumber,
                AccountType = account.Type.ToString(),
                StartBalance = startingBalance,
                EndBalance = endingBalance,
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                GeneratedAt = DateTime.Now,
                Transactions = processedTransactions
            };

            return statement;
        }
    }
}
