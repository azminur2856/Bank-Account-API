using DAL.EF.Tables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Interfaces
{
    public interface IAccountFeatures
    {
        Account GetLastAccount();
        List<Account> GetByUserId(int userId);
    }
}
