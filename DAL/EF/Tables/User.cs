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
    public class User
    {
        [Key]
        public int UserId { get; set; }

        [Required]
        [StringLength(100)]
        [Column(TypeName = "VARCHAR")]
        public string FullName { get; set; }

        [Required]
        [StringLength(100)]
        [Column(TypeName = "VARCHAR")]
        public string Email { get; set; }

        [Required]
        [StringLength(15)]
        [Column(TypeName = "VARCHAR")]
        public string PhoneNumber { get; set; }

        [Required]
        [Column(TypeName = "VARCHAR")]
        public string PasswordHash { get; set; }

        [Required]
        public UserRole Role { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime? UpdatedAt { get; set; }

        public virtual List<Account> Accounts { get; set; }

        public virtual List<Transaction> Transactions { get; set; }

        public virtual List<AuditLog> AuditLogs { get; set; }

        public virtual List<Token> Tokens { get; set; }
    }
}
