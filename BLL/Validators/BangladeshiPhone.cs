using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BLL.Validators
{
    public class BangladeshiPhone : ValidationAttribute
    {
        private static readonly Regex regex = new Regex(@"^01[3-9]\d{8}$", RegexOptions.Compiled);

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null)
            {
                return new ValidationResult("Phone number is required.");
            }

            string phone = value.ToString();

            if (!regex.IsMatch(phone))
            {
                return new ValidationResult("Invalid Bangladeshi phone number. Valid Format: 01XXXXXXXXX (11 digits).");
            }

            return ValidationResult.Success;
        }
    }
}
