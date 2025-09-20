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
    public class Verification
    {
        [Key]
        public int VerificationId { get; set; }

        [Required]
        [Column(TypeName = "VARCHAR")]
        public string Code { get; set; }

        [Required]
        public VerificationType Type { get; set; }

        [Required]
        public bool IsUsed { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Required]
        public DateTime ExpireAt { get; set; }

        [ForeignKey("User")]
        public int UserId { get; set; }

        public virtual User User { get; set; }
    }
}
