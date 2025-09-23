using DnsClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Utility
{
    public static class Helper
    {
        public static (bool isValid, T parsedEnum, List<string> validNames) ParseEnum<T>(string value) where T : struct, IConvertible
        {
            if (!typeof(T).IsEnum)
            {
                throw new ArgumentException("T must be an enumerated type");
            }

            var normalizedInput = new string(value
                .Where(char.IsLetterOrDigit)
                .ToArray())
                .ToLower();

            var validNames = Enum.GetNames(typeof(T)).ToList();

            foreach (var name in validNames)
            {
                var normalizedEnumName = new string(name
                    .Where(char.IsLetterOrDigit)
                    .ToArray())
                    .ToLower();

                if (normalizedInput == normalizedEnumName)
                {
                    return (true, (T)Enum.Parse(typeof(T), name, true), null);
                }
            }

            return (false, default(T), validNames);
        }

        public static string GenerateToken(int byteLength = 48)
        {
            // 48 random bytes ≈ 64 base64url chars
            byte[] randomBytes = new byte[byteLength];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomBytes);
            }

            string base64 = Convert.ToBase64String(randomBytes);

            return base64.Replace("+", "-")
                         .Replace("/", "_")
                         .Replace("=", "");
        }

        public static string GenerateOtp(int byteLength = 4)
        {
            using (var rng = RandomNumberGenerator.Create())
            {
                var bytes = new byte[byteLength];
                rng.GetBytes(bytes);

                int value = BitConverter.ToInt32(bytes, 0) & 0x7FFFFFFF;
                return (value % 1_000_000).ToString("D6"); // Always 6 digits
            }
        }

        public static bool DomainHasMxRecord(string email)
        {
            try
            {
                var domain = email.Split('@')[1];
                var lookup = new LookupClient();
                var result = lookup.Query(domain, QueryType.MX);

                return result.Answers.MxRecords().Any();
            }
            catch
            {
                return false;
            }
        }
    }
}
