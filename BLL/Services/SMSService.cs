using BLL.DTOs;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime;
using System.Text;
using System.Threading.Tasks;
using static AutoMapper.Internal.ExpressionFactory;

namespace BLL.Services
{
    public class SmsService
    {
        private readonly SmsSecretDTO secret;
        private readonly HttpClient httpClient = new HttpClient();

        public SmsService(SmsSecretDTO secret)
        {
            this.secret = secret;
        }

        public async Task<bool> SendSMSAsync(string numbers, string Message)
        {
            var paylod = new
            {
                api_key = secret.ApiKey,
                senderid = secret.SenderId,
                number = numbers,
                message = Message
            };

            var jesonPaylod = JsonConvert.SerializeObject(paylod);
            var content = new StringContent(jesonPaylod, Encoding.UTF8, "application/json");

            var response = await httpClient.PostAsync(secret.ApiUrl, content);

            if(!response.IsSuccessStatusCode) return false;

            var respString = await response.Content.ReadAsStringAsync();

            return respString.Contains("202");
        }
    }
}
