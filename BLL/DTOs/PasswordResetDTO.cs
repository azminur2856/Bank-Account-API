using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.DTOs
{
    public class PasswordResetDTO
    {
        public string ResetOtp { get; set; }
        public string NewPassword { get; set; }
    }
}
