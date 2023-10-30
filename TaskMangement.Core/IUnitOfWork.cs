using TaskMangement.Core.Interfaces;
using TaskMangement.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskMangement.Core
{
    public interface IUnitOfWork : IDisposable
    {
        IBaseRepository<User> Users { get; }
        IBaseRepository<TaskMangement.Core.Models.Task> Tasks { get; }
        IBaseRepository<UserTask> UserTasks { get; }


        int Complete();
    }
}