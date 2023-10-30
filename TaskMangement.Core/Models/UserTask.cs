using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskMangement.Core.Models
{
    public class UserTask
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public User User { get; set; }
        public int TaskId { get; set; }
        public Task Task { get; set; }
    }
}
