using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.DTOs
{
    public class UserStatusChangeDTO
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public bool IsActive { get; set; }
    }
}
