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
    public class AccountPolicy
    {
        [Key]
        public int PolicyId { get; set; }

        [Required]
        public AccountType AccountType { get; set; }

        [Required]
        [Column(TypeName = "DECIMAL")]
        public decimal MinimumBalance { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? UpdatedAt { get; set; }
    }
}
