using BLL.DTOs;
using BLL.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace BLL.Services
{
    internal class SMSService : ISMSService
    {
        private readonly SecretSettingsDTO secret;
        private readonly HttpClient httpClient;

        public SMSService()
        {
            string path = Path.Combine(AppContext.BaseDirectory, "secretsettings.json");

            if (!File.Exists(path))
            {
                throw new FileNotFoundException("The configuration file 'secretsettings.json' was not found.");
            }

            var json = File.ReadAllText(path);

            var jsonDoc = JsonDocument.Parse(json);
            var secretSettingsJeson = jsonDoc.RootElement.GetProperty("SecretSettings").GetRawText();
            secret = JsonSerializer.Deserialize<SecretSettingsDTO>(secretSettingsJeson);
        }
        public async Task<bool> SendSMSAsync(string numbers, string message)
        {
            var paylod = new
            {
                api_key = secret.SMSApiKey,
                senderid = secret.SMSSenderId,
                number = numbers,
                message = message
            };

            var jesonPaylod = JsonSerializer.Serialize(paylod);
            var content = new StringContent(jesonPaylod, Encoding.UTF8, "application/json");

            var response = await httpClient.PostAsync(secret.SMSApiUrl, content);
            return response.IsSuccessStatusCode;
        }
    }
}
