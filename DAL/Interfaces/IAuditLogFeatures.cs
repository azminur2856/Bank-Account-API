using DAL.EF.Tables;
using DAL.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Interfaces
{
    public interface IAuditLogFeatures
    {
        List<AuditLog> GetByType(AuditLogType type);
        List<AuditLog> GetByDateRange(DateTime startDate, DateTime endDate);
        List<AuditLog> GetByUserEmail(string email);
        List<AuditLog> GetAll(int page, int pageSize, string sortBy, bool isDescending);
    }
}
