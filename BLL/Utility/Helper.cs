using System;
using System.Collections.Generic;
using System.Linq;
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
    }
}
