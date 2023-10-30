using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using TaskMangement.Api.IServices;
using TaskMangement.Core;
using TaskMangement.Core.DTOs;
using TaskMangement.Core.Helpers;
using TaskMangement.Core.Models;
using TaskMangement.EF;

namespace TaskMangement.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration configuration;
        private readonly JWT _jwt;
        private readonly IAuthServices _Auth;
        private readonly IUnitOfWork _UnitOfWork;
       
        public UserController(ApplicationDbContext context, UserManager<User> userManager, RoleManager<IdentityRole> roleManager,
            IConfiguration configuration, IOptions<JWT> jwt,IAuthServices Auth,IUnitOfWork UnitOfWork)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
            this.configuration = configuration;
            _jwt = jwt.Value;
            _Auth = Auth;
            _UnitOfWork = UnitOfWork;

        }
        [HttpPost("Register")]
        public async Task<IActionResult> Registeration([FromForm] RegisterDTO user)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var result = await _Auth.RegisterAsync(user);
            if(result.IsAuthenticated)return Ok(result);
            else return BadRequest(result);
        }
        [HttpPost("Login")]
        public async Task<IActionResult> loginAsync([FromForm] LoginDTO user)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var result = await _Auth.loginAsync(user);
            return Ok(result);
        }
        [HttpGet("GetAllUsers")]
        [Authorize(Roles ="Admin")]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = _userManager.Users.ToList();
            if (users == null)return BadRequest(ModelState);
            return Ok(users);
        }
        /*[HttpPost]
        public async Task<IActionResult> CreateUser([FromForm] RegisterDTO DTO)
        {

            //var user = _mapper.Map<User>(DTO);
            using var dataStream = new MemoryStream();
            await DTO.ProfilePicture.CopyToAsync(dataStream);

            var user = new User
            {
                UserName = DTO.Name,
                Name = DTO.Name,
                PhoneNumber = DTO.PhoneNumber,
                Email = DTO.Email,
                ProfilePicture = dataStream.ToArray()
            };
            var result = await _userManager.CreateAsync(user, DTO.Password);
            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }
            await _userManager.AddToRoleAsync(user, "Admin");
            await _context.SaveChangesAsync();

            return Ok(user);
        }*/
    }
}
