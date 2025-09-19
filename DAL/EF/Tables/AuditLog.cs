using DAL.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.EF.Tables
{
    public class AuditLog
    {
        [Key]
        public int LogId { get; set; }

        [Required]
        [ForeignKey("User")]
        public int UserId { get; set; }

        [Required]
        public AuditLogType Type { get; set; }

        [Column(TypeName = "TEXT")]
        public string Details { get; set; }

        public DateTime Timestamp { get; set; } = DateTime.Now;

        public virtual User User { get; set; }
    }
}
