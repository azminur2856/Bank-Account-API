using DAL.EF.Tables;
using DAL.Enums;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BLL.Services
{
    public class TransactionDTO
    {
        public int TransactionId { get; set; }
        public int? SourceAccountId { get; set; }
        public int? DestinationAccountId { get; set; }
        [Required]
        public decimal Amount { get; set; }
        public decimal? Fees { get; set; }
        [Required]
        public TransactionType Type { get; set; }
        public TransactionType? SourceType { get; set; }
        public TransactionType? DestinationType { get; set; }
        [Required]
        public int PerformedBy { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}