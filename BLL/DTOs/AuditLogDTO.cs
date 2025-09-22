using DAL.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.DTOs
{
    public class AuditLogDTO
    {
        public int LogId { get; set; }
        public int UserId { get; set; }
        public AuditLogType Type { get; set; }
        public string Details { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
