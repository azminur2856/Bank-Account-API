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
    internal class TransactionRepo : IRepo<Transaction, int, bool>
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

        public bool Update(Transaction obj)
        {
            throw new NotImplementedException();
        }
    }
}
