using AutoMapper;
using BLL.DTOs;
using BLL.Utility;
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

        public static AccountDTO Create(AccountDTO account)
        {
            var lastAccount = DataAccessFactory.AccountFeaturesData().GetLastAccount();
            var newAccountNumber = Helper.GenerateNewAccountNumber(lastAccount);

            var mapper = GetMapper();
            var newAccountEntity = mapper.Map<Account>(account);
            newAccountEntity.AccountNumber = newAccountNumber;
            newAccountEntity.CreatedAt = DateTime.Now;
            newAccountEntity.IsActive = true;

            var createdAccount = DataAccessFactory.AccountData().Create(newAccountEntity);

            return mapper.Map<AccountDTO>(createdAccount);
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
