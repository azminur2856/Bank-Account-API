using BLL.Validators;
using DAL.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.DTOs
{
    public class UserDTO
    {
        public int UserId { get; set; }
        [Required]
        [StringLength(100, ErrorMessage = "Full name cannot be more than 100 characters.")]
        public string FullName { get; set; }
        [Required]
        [StringLength(100, ErrorMessage = "Email cannot be more than 100 characters.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string Email { get; set; }
        public bool IsEmailVerified { get; set; }
        [Required(ErrorMessage = "Phone number is required.")]
        [BangladeshiPhone]
        public string PhoneNumber { get; set; }
        public bool IsPhoneNumberVerified { get; set; }
        [Required(ErrorMessage = "Password is required.")]
        public string PasswordHash { get; set; }
        public UserRole Role { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
