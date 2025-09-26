using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.DTOs
{
    public class StatementDTO
    {
        public string FullName { get; set; }
        public string Address { get; set; }
        public string AccountNumber { get; set; }
        public string AccountType { get; set; }
        public decimal StartBalance { get; set; }
        public decimal EndBalance { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime GeneratedAt { get; set; }
        public List<StatementTransactionDTO> Transactions { get; set; }
    }
}
