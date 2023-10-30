using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskMangement.Core.Models
{
    public class User:IdentityUser
    {
        /// <summary>
        /// public string Name { get; set; }
        /// </summary>
        public byte[] ProfilePicture { get; set; }
        public ICollection<UserTask> UserTasks { get; set; }
    }
}
