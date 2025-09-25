using DAL.EF;
using DAL.EF.Tables;
using DAL.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Repos
{
    internal class AccountRepo : IRepo<Account, string, Account>, IAccountFeatures
    {
        BANKContext db;
        public AccountRepo()
        {
            db = new BANKContext();
        }
        public Account Create(Account obj)
        {
            db.Accounts.Add(obj);
            db.SaveChanges();
            return obj;
        }

        public bool Delete(string id)
        {
            throw new NotImplementedException();
        }

        public Account Get(string id)
        {
            var account = (from a in db.Accounts
                           where a.AccountNumber.Equals(id)
                           select a).SingleOrDefault();
            return account;
        }

        public List<Account> Get()
        {
            return db.Accounts.ToList();
        }

        public List<Account> GetByUserId(int userId)
        {
            return db.Accounts
                     .Where(a => a.UserId == userId)
                     .ToList();
        }

        public Account GetLastAccount()
        {
            return db.Accounts
                     .OrderByDescending(a => a.AccountNumber)
                     .FirstOrDefault();
        }

        public Account Update(Account obj)
        {
            var exobj = Get(obj.AccountNumber);
            db.Entry(exobj).CurrentValues.SetValues(obj);
            db.SaveChanges();
            return obj;
        }
    }
}
