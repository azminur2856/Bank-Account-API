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
        EmailVerification = 1,
        PhoneVerification,
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
        UserCreated = 1,
        UserLogin,
        UserLogout,
        AccountCreated,
        TransactionPerformed,
        UpdateAccountType,
        PasswordChanged,
        RoleChangedToEmployee,
        CompleateRegistrationActiveAccountAndVerifyEmail,
        EmailVrified,
        PhoneVerified,
        PasswordResetEmailOtpRequested,
        PasswordResetSmsOtpRequested,
        PasswordResetCompleted,
        EmailVerificationLinkSent,
        PhoneVerificationCodeSent,
        PresentAddressAdded,
        PermanentAddressAdded,
        PresentAddressUpdated,
        AddressVerified,
        RoleChangedToCustomer,
        UserActivated,
        UserDeactivated,
        Deposit,
        Withdrawal,
        Transfer,
        SystemTransaction,
        AccountCreatedByAdmin,
        AccountCreatedByEmployee,
        AccountCreatedByCustomer,
        AccountActivatedByAdmin,
        AccountActivatedByEmployee
    }

    public enum AddressType
    {
        Present = 1,
        Permanent
    }

    public class Enums
    {
    }
}
