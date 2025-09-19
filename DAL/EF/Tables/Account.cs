using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.EF.Tables
{
    public class Account
    {
        [Key]
        public int AccountId { get; set; }

        [Required]
        [ForeignKey("User")]
        public int UserId { get; set; }

        [Required]
        [StringLength(20)]
        [Column(TypeName = "VARCHAR")]
        public string AccountNumber { get; set; }

        [Required]
        [Column(TypeName = "DECIMAL")]
        public decimal Balance { get; set; } = 0;

        [Required]
        public bool IsActive { get; set; } = false;

        [ForeignKey("CreatedByUser")]
        public int? CreatedBy { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? UpdatedAt { get; set; }

        public virtual User User { get; set; }

        public virtual User CreatedByUser { get; set; }
    }
}
