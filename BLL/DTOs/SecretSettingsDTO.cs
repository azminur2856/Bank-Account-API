using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.DTOs
{
    internal class SecretSettingsDTO
    {
        // Properties for email configuration
        public string FromEmail { get; set; }
        public string AppPassword { get; set; }
        public string Host { get; set; }
        public int Port { get; set; }

        // Properties for SMS configuration
        public string SMSApiUrl { get; set; }
        public string SMSApiKey { get; set; }
        public string SMSSenderId { get; set; }
    }
}
