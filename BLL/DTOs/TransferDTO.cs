using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.DTOs
{
    public class TransferDTO
    {
        [Required]
        public string SourceAccountNumber { get; set; }

        [Required]
        public string DestinationAccountNumber { get; set; }

        [Required]
        [Range(50, 25000, ErrorMessage = "Transfer amount must be between 50 and 250,000.")]
        public decimal Amount { get; set; }

        public decimal? Fees { get; set; } = 0;
    }
}
