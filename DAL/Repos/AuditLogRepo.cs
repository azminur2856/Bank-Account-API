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
    internal class AuditLogRepo : IRepo<AuditLog, int, bool>
    {
        BANKContext db;
        public AuditLogRepo()
        {
            db = new BANKContext();
        }   
        public bool Create(AuditLog obj)
        {
            db.AuditLogs.Add(obj);
            return db.SaveChanges() > 0;
        }

        public bool Delete(int id)
        {
            throw new NotImplementedException();
        }

        public AuditLog Get(int id)
        {
            throw new NotImplementedException();
        }

        public List<AuditLog> Get()
        {
            return db.AuditLogs.ToList();
        }

        public bool Update(AuditLog obj)
        {
            throw new NotImplementedException();
        }
    }
}
