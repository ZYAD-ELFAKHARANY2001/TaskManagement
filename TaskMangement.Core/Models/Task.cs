using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskMangement.Core.Models
{
    public class Task
    {
        public int TaskId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime DueDate { get; set; } = DateTime.Now;
        public bool IsFinished { get; set; }
        [ForeignKey("User")]
        public string CreatorId { get; set; }
        public ICollection<User> AssignedUsers { get; set; } = new List<User>();
    }
}
