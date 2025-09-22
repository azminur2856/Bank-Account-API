using DAL.EF.Tables;
using DAL.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Interfaces
{
    public interface IVerificationFeatures
    {
        Verification GetByCodeAndType(string code, VerificationType type);
    }
}
