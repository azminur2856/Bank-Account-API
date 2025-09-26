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

        public List<AuditLog> GetAll(int page, int pageSize, string sortBy, bool isDescending)
        {
            var query = db.AuditLogs.AsQueryable();

            switch (sortBy.ToLowerInvariant())
            {
                case "logid":
                    query = isDescending ? query.OrderByDescending(l => l.LogId) : query.OrderBy(l => l.LogId);
                    break;
                case "userid":
                    query = isDescending ? query.OrderByDescending(l => l.UserId) : query.OrderBy(l => l.UserId);
                    break;
                case "type":
                    query = isDescending ? query.OrderByDescending(l => l.Type) : query.OrderBy(l => l.Type);
                    break;
                case "timestamp":
                default:
                    query = isDescending ? query.OrderByDescending(l => l.Timestamp) : query.OrderBy(l => l.Timestamp);
                    break;

            }

            query = query
                .Skip((page - 1) * pageSize)
                .Take(pageSize);
            return query.ToList();
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
