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
        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be a positive value.")]
        public decimal Amount { get; set; }

        [Range(0.0, double.MaxValue, ErrorMessage = "Fees must be a non-negative value.")]
        public decimal? Fees { get; set; } = 0;
    }
}
