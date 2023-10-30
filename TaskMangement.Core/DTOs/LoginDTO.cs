using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskMangement.Core.DTOs
{
    public class LoginDTO
    {
        [MaxLength(40)]
        public string UserName { get; set; }
        [MaxLength(30)]
        public string Password { get; set; }
    }
}
