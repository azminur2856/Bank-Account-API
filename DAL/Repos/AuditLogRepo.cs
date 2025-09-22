using DAL.EF;
using DAL.EF.Tables;
using DAL.Enums;
using DAL.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Repos
{
    internal class AuditLogRepo : IRepo<AuditLog, int, bool>, IAuditLogFeatures
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

        public List<AuditLog> GetByDateRange(DateTime startDate, DateTime endDate)
        {
            var logs = (from l in db.AuditLogs
                        where l.Timestamp >= startDate && l.Timestamp <= endDate
                        select l).ToList();
            return logs;
        }

        public List<AuditLog> GetByType(AuditLogType type)
        {
            var logs = db.AuditLogs.Where(a => a.Type == type).ToList();
            return logs;
        }

        public List<AuditLog> GetByUserEmail(string email)
        {
            var logs = (from l in db.AuditLogs
                        join u in db.Users on l.UserId equals u.UserId
                        where u.Email.Equals(email)
                        select l).ToList();
            return logs;
        }

        public bool Update(AuditLog obj)
        {
            throw new NotImplementedException();
        }
    }
}
