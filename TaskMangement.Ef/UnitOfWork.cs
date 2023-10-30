using TaskMangement.Core;
using TaskMangement.Core.Interfaces;
using TaskMangement.Core.Models;
using TaskMangement.EF.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskMangement.EF;

namespace TaskMangement.EF
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;

        public IBaseRepository<TaskMangement.Core.Models.Task> Tasks { get; private set; }
        public IBaseRepository<TaskMangement.Core.Models.User> Users { get; private set; }
        public IBaseRepository<TaskMangement.Core.Models.UserTask> UserTasks { get; private set; }


        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;

            var Tasks = new BaseRepository<TaskMangement.Core.Models.Task>(_context);
            var Users = new BaseRepository<User>(_context);
            var UserTasks = new BaseRepository<UserTask>(_context);

        }

        public int Complete()
        {
            return _context.SaveChanges();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}