using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskMangement.Core.DTOs
{
    public class RegisterDTO
    {
        [MaxLength(50)]
        public string Name { get; set; }
        public string Email { get; set; }
        public IFormFile? ProfilePicture { get; set; }
        public string? PhoneNumber { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        [Compare("Password")]
        public string ConfirmPassword { get; set; }
    }
}
