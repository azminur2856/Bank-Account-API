using BLL.DTOs;
using BLL.Interfaces;
using BLL.Services;
using DAL;
using DAL.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL
{
    public class ServiceFactory
    {
        public static IEmailService EmailService { get; private set; }
        public static ISmsService SmsService { get; private set; }
        public static string ApiBaseUrl { get; private set; }
        public static string BankMasterAccountNumber { get; private set; }
        public static decimal TransferFee { get; private set; }
        public static Dictionary<AccountType, decimal> MinimumBalances { get; private set; }

        public static void Init(EmailSecretDTO emailSecret, SmsSecretDTO smsSecret, string apiBaseUrl, string bankMasterAccountNumber, decimal transferFee)
        {
            EmailService = new EmailService(emailSecret);
            SmsService = new SmsService(smsSecret);
            ApiBaseUrl = apiBaseUrl;
            BankMasterAccountNumber = bankMasterAccountNumber;
            TransferFee = transferFee;
            MinimumBalances = DataAccessFactory.AccountPolicyFeaturesData().GetMinimumBalances();
        }
    }
}
