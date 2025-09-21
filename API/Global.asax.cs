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
        public static EmailService EmailService;
        public static SmsService SmsService;
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
            System.Diagnostics.Debug.WriteLine($"SMTP_USER: {emailSecret.User}");


            // Load SMS configuration from environment variables
            var smsSecret = new SmsSecretDTO
            {
                ApiUrl = Env.GetString("SMS_API_URL"),
                ApiKey = Env.GetString("SMS_API_KEY"),
                SenderId = Env.GetString("SMS_SENDER_ID")
            };

            EmailService = new EmailService(emailSecret);
            SmsService = new SmsService(smsSecret);

            GlobalConfiguration.Configure(WebApiConfig.Register);
        }
    }
}
