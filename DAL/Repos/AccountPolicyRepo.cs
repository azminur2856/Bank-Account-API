using DAL.EF;
using DAL.Enums;
using DAL.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Repos
{
    internal class AccountPolicyRepo : IAccountPolicyFeatures
    {
        BANKContext db;
        public AccountPolicyRepo()
        {
            db = new BANKContext();
        }
        public Dictionary<AccountType, decimal> GetMinimumBalances()
        {
            return db.AccountPolicies.ToDictionary(
                ap => ap.AccountType,
                ap => ap.MinimumBalance
            );
        }
    }
}
