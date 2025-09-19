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
    public class Transaction
    {
        [Key]
        public int TransactionId { get; set; }

        [ForeignKey("SourceAccount")]
        public int? SourceAccountId { get; set; }

        [ForeignKey("DestinationAccount")]
        public int? DestinationAccountId { get; set; }

        [Required]
        [Column(TypeName = "DECIMAL")]
        public decimal Amount { get; set; }

        [Required]
        public TransactionType Type { get; set; }

        [Required]
        [ForeignKey("PerformedByUser")]
        public int PerformedBy { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public virtual Account SourceAccount { get; set; }
        public virtual Account DestinationAccount { get; set; }
        public virtual User PerformedByUser { get; set; }
    }
}
