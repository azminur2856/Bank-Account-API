using DAL.EF.Tables;
using DAL.Interfaces;
using DAL.Repos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public class DataAccessFactory
    {
        public static IRepo<User, int, bool> UserData()
        {
            return new UserRepo();
        }

        public static IUserFeatures UserFeaturesData()
        {
            return new UserRepo();
        }

        public static IRepo<Account, string, Account> AccountData()
        {
            return new AccountRepo();
        }

        public static IAccountFeatures AccountFeaturesData()
        {
            return new AccountRepo();
        }

        public static IRepo<Transaction, int, bool> TransactionData()
        {
            return new TransactionRepo();
        }

        public static ITransactionFeatures TransactionFeaturesData()
        {
            return new TransactionRepo();
        }

        public static IRepo<AuditLog, int, bool> AuditLogData()
        {
            return new AuditLogRepo();
        }

        public static IAuditLogFeatures AuditLogFeaturesData()
        {
            return new AuditLogRepo();
        }

        public static IRepo<Token, string, Token> TokenData()
        {
            return new TokenRepo();
        }

        public static IRepo<Verification, string, bool> VerificationData()
        {
            return new VerificationRepo();
        }

        public static IVerificationFeatures VerificationFeaturesData()
        {
            return new VerificationRepo();
        }

        public static IRepo<Address, int, bool> AddressData()
        {
            return new AddressRepo();
        }

        public static IAddressFeatures AddressFeaturesData()
        {
            return new AddressRepo();
        }
    }
}
