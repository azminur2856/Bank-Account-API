using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Interfaces
{
    public interface ISmsService
    {
        Task<bool> SendSMSAsync(string number, string message);
    }
}
