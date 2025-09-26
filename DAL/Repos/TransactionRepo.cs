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
    internal class TransactionRepo : IRepo<Transaction, int, bool> , ITransactionFeatures
    {
        BANKContext db;
        public TransactionRepo()
        {
            db = new BANKContext();
        }

        public bool Create(Transaction obj)
        {
            db.Transactions.Add(obj);
            return db.SaveChanges() > 0;
        }

        public bool Delete(int id)
        {
            throw new NotImplementedException();
        }

        public Transaction Get(int id)
        {
            throw new NotImplementedException();
        }

        public List<Transaction> Get()
        {
            return db.Transactions.ToList();
        }

        public List<Transaction> GetByAccountAndDateRange(int accountId, DateTime startDate, DateTime endDate)
        {
            return db.Transactions
                     .Where(t => (t.SourceAccountId == accountId || t.DestinationAccountId == accountId) &&
                                 t.CreatedAt >= startDate && t.CreatedAt <= endDate)
                     .ToList();
        }

        public bool Update(Transaction obj)
        {
            throw new NotImplementedException();
        }
    }
}
