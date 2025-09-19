using AutoMapper;
using BLL.DTOs;
using DAL;
using DAL.EF.Tables;
using System;
using System.Collections.Generic;
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

        private static string GenerateNewAccountNumber()
        {
            var lastAccount = DataAccessFactory.AccountFeaturesData().GetLastAccount();
            string prefix = "2313";
            long startNumber = 1;

            if (lastAccount == null)
            {
                return prefix + startNumber.ToString("D9");
            }

            string lastNumberStr = lastAccount.AccountNumber.Substring(4);
            long lastNumber = long.Parse(lastNumberStr);
            long newNumber = lastNumber + 1;

            return prefix + newNumber.ToString("D9");
        }

        public static AccountDTO Create(AccountDTO account)
        {
            account.UserId = 1; //Face the current login user id if user is created it or if employee create then face the user who use the account
            account.AccountNumber = GenerateNewAccountNumber();
            account.Balance = 0;
            account.IsActive = false;
            account.CreatedBy = 1; //Face the current login user id
            account.CreatedAt = DateTime.Now;

            var accountM = GetMapper().Map<Account>(account);
            return GetMapper().Map<AccountDTO>(DataAccessFactory.AccountData().Create(accountM));
        }

        public static List<AccountDTO> Get()
        {
            return GetMapper().Map<List<AccountDTO>>(DataAccessFactory.AccountData().Get());
        }

        public static AccountDTO Get(string AccountNumber)
        {
            return GetMapper().Map<AccountDTO>(DataAccessFactory.AccountData().Get(AccountNumber));
        }

    }
}
