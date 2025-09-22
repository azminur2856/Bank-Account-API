using BLL.DTOs;
using BLL.Interfaces;
using BLL.Services;
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

        public static void Init(EmailSecretDTO emailSecret, SmsSecretDTO smsSecret, string apiBaseUrl)
        {
            EmailService = new EmailService(emailSecret);
            SmsService = new SmsService(smsSecret);
            ApiBaseUrl = apiBaseUrl;
        }
    }
}
