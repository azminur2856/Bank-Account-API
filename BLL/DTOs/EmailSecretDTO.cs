using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.DTOs
{
    public class EmailSecretDTO
    {
        public string Host { get; set; }
        public int Port { get; set; }
        public string User { get; set; }
        public string AppPassword { get; set; }
    }
}
