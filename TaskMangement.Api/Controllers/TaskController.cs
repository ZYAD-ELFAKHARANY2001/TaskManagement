using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TaskMangement.Core;
using TaskMangement.Core.Consts;
using TaskMangement.Core.Models;

namespace TaskMangement.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TaskController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        [HttpPost("AddOne")]
        [Authorize]
        public IActionResult AddOne([FromForm] Core.Models.Task task)
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var TaskCreated = _unitOfWork.Tasks.Add(new Core.Models.Task 
            {
                Title = task.Title,
                DueDate = task.DueDate,
                IsFinished = task.IsFinished,
                AssignedUsers = task.AssignedUsers,
                Description = task.Description,
                CreatorId = userId
            });
            _unitOfWork.Complete();
            return Ok(TaskCreated);
        }

        [HttpPost("AddRangeOfUsers")]
        [Authorize]
        public IActionResult AddRangeOfUsers(int id,[FromForm]IEnumerable<Core.Models.User> users)
        {
            var task = _unitOfWork.Tasks.GetById(id);
            
            if (task != null) 
            {
                foreach (var user in users)
                {
                    task.AssignedUsers.Add(user);
                }
                _unitOfWork.Complete();
                return Ok(task);
            }
            else
            {
                return BadRequest();
            }
            
        }
        [HttpPut]
        [Authorize]
        public IActionResult SetFinishedOrNot(int id,bool IsFinished=false) 
        {
            var task = _unitOfWork.Tasks.GetById(id);
            if (task != null) 
            {
                task.IsFinished = IsFinished;
                _unitOfWork.Complete();
                return Ok();
            }
            return BadRequest();
        }
        [HttpGet("CreatedTasks")]
        [Authorize]
        public IActionResult GetCreatedTasks()
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var task = _unitOfWork.Tasks.FindAll(t => t.CreatorId == userId);
            if (task != null)
            {
                return Ok(task);
            }
            return BadRequest();
        }
        [HttpGet("AssignedTasks")]
        [Authorize]
        public IActionResult GetAssignedTasks()
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var task = _unitOfWork.Tasks.GetAll();
            _unitOfWork.Tasks.FoundOrNo(task, userId);
            if (task != null)
            {
                return Ok();
            }
            return BadRequest();
        }
        [HttpDelete]
        [Authorize]
        public IActionResult DeleteTask(int id)
        {
            var task = _unitOfWork.Tasks.GetById(id);
            if (task != null)
            {
                _unitOfWork.Tasks.Delete(task);
                return Ok();
            }
            return BadRequest();
        }
        [HttpGet("GetOrdered")]
        [Authorize]
        public IActionResult GetOrdered()
        {
            return Ok(_unitOfWork.Tasks.FindAll(null, null, b => b.DueDate, OrderBy.Ascending));
        }

        [HttpGet("Admin")]
        [Authorize(Roles ="Admin")]
        public IActionResult GetAllTasksWithUsers()
        {
            var task = _unitOfWork.Tasks.GetAll();
            if (task != null)
            {
                return Ok();
            }
            return BadRequest("There are No tasks to get");

        }
    }
}
