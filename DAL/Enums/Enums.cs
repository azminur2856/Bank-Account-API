using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Enums
{
    public enum UserRole
    {
        Admin = 1,
        Employee,
        Customer
    }

    public enum VerificationType
    {
        RegistrationVerification = 1,
        PhoneVerification,
        EmailVerification,
        PasswordReset
    }

    public enum AccountType
    {
        Master = 1,
        Savings,
        Current
    }

    public enum TransactionType
    {
        Deposit = 1,
        Withdrawal,
        Transfer,
        SystemCredit,
        SystemDebit
    }

    public enum AuditLogType
    {
        UserLogin = 1,
        UserLogout,
        AccountCreated,
        TransactionPerformed,
        PasswordChanged,
        RoleChanged
    }
    public class Enums
    {
    }
}
