using System;
using System.Collections.Generic;

namespace TaskMangement.Core.Models
{
    public class AuthModel
    {
        public string Message { get; set; }
        public bool IsAuthenticated { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }
        public string Token { get; set; }
        public DateTime ExpireOn { get; set; }
    }
}