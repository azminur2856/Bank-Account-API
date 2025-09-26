using BLL;
using BLL.DTOs;
using BLL.Services;
using DotNetEnv;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Routing;

namespace API
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            // Get the path to the .env file in the application root
            string envPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ".env");

            // Check if .env file is present
            if (!System.IO.File.Exists(envPath))
            {
                throw new InvalidOperationException($".env file not found at path: {envPath}");
            }

            // Load .env file
            Env.Load(envPath);

            // Load email configuration from environment variables
            var emailSecret = new EmailSecretDTO
            {
                Host = Env.GetString("SMTP_HOST"),
                Port = Env.GetInt("SMTP_PORT"),
                User = Env.GetString("SMTP_USER"),
                AppPassword = Env.GetString("SMTP_APP_PASSWORD")
            };

            // Load SMS configuration from environment variables
            var smsSecret = new SmsSecretDTO
            {
                ApiUrl = Env.GetString("SMS_API_URL"),
                ApiKey = Env.GetString("SMS_API_KEY"),
                SenderId = Env.GetString("SMS_SENDER_ID")
            };

            // Get API base URL from environment variables
            string apiBaseUrl = Env.GetString("API_BASE_URL");

            // Get Bank Master Account Number from environment variables
            string bankMasterAccountNumber = Env.GetString("BANK_MASTER_ACCOUNT_NUMBER");

            // Get Transfer Fee from environment variables
            string transferFeeString = Env.GetString("TRANSFER_FEE");
            decimal transferFee = Convert.ToDecimal(transferFeeString);
            // Get Monthly Maintenance Fee from environment variables
            string semiAnnualMaintenanceFeeString = Env.GetString("SEMI_ANNUAL_MAINTENANCE_FEE");
            decimal semiAnnualMaintenanceFee = Convert.ToDecimal(semiAnnualMaintenanceFeeString);

            ServiceFactory.Init(emailSecret, smsSecret, apiBaseUrl, bankMasterAccountNumber, transferFee, semiAnnualMaintenanceFee);

            GlobalConfiguration.Configure(WebApiConfig.Register);
        }
    }
}
