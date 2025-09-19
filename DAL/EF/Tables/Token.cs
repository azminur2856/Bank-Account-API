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
    public class Token
    {
        [Key]
        public int TokenId { get; set; }

        [Required]
        [Column(TypeName = "VARCHAR")]
        public string TokenKey { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime? ExpireAt { get; set; }

        [ForeignKey("User")]
        public int UserId { get; set; }

        public virtual User User { get; set; }
    }
}
