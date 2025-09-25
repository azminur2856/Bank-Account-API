using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.DTOs
{
    public class WithdrawalDTO
    {
        [Required]
        public string SourceAccountNumber { get; set; }

        [Required]
        [Range(500, 50000, ErrorMessage = "Withdrawa amount must be between 500 and 50,000.")]
        public decimal Amount { get; set; }

        public decimal? Fees { get; set; } = 0;
    }
}
