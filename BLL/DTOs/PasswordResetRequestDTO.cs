using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.DTOs
{
    public class PasswordResetRequestDTO
    {
        [Required]
        [StringLength(100, ErrorMessage = "Email cannot be more than 100 characters.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string Email { get; set; }
        public string Type { get; set; } = "Email";
    }
}
