using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.DTOs
{
    public class StatementTransactionDTO
    {
        public string Date { get; set; }
        public string Type { get; set; }
        public string Details { get; set; }
        public decimal Debit { get; set; }
        public decimal Credit { get; set; }
    }
}
