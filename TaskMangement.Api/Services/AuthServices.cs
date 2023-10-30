using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System;
using TaskMangement.Api.IServices;
using TaskMangement.Core.DTOs;
using TaskMangement.Core.Helpers;
using TaskMangement.Core.Models;
using TaskMangement.Core;

namespace TaskMangement.Api.Services
{
    public class AuthServices:IAuthServices
    {
        private readonly UserManager<User> _UserManager;
        private readonly JWT _jwt;
        private readonly RoleManager<IdentityRole> _RoleManager;
        private readonly IUnitOfWork _UnitOfWork;
        public AuthServices(UserManager<User> usermanger, IOptions<JWT> jwt, IUnitOfWork unitofwork, RoleManager<IdentityRole> rolemanager)
        {
            _UserManager = usermanger;
            _jwt = jwt.Value;
            _UnitOfWork = unitofwork;
            _RoleManager = rolemanager;
        }
        public async Task<AuthModel> RegisterAsync(RegisterDTO user)
        {
            AuthModel NotValidData = await registerationValidation(user);
            if (NotValidData.IsAuthenticated == false)
            {
                return NotValidData;
            }

            using var dataStream = new MemoryStream();
            await user.ProfilePicture.CopyToAsync(dataStream);
            User NewAccount = new User()
            {
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                UserName = user.Name,
                ProfilePicture = dataStream.ToArray()
            };

            var result = await _UserManager.CreateAsync(NewAccount, user.Password);
            await _UserManager.AddToRoleAsync(NewAccount, "Admin");
            if (result.Succeeded == false)
            {
                var errors = string.Empty;
                foreach (var error in result.Errors)
                {
                    errors += $"{error.Description},";
                }
                errors.Remove(errors.Length - 1, 1);
                await _UserManager.DeleteAsync(NewAccount);
                
                return new AuthModel() {IsAuthenticated=false, Message = errors };
            }

            _UnitOfWork.Complete();
            var JwtSecurityToken = await CreateJwtToken(NewAccount);
            var RolesList = await _UserManager.GetRolesAsync(NewAccount);
            var role = RolesList.FirstOrDefault();
            return new AuthModel()
            {
                ExpireOn = JwtSecurityToken.ValidTo,
                IsAuthenticated = true,
                Role = role,
                Token = new JwtSecurityTokenHandler().WriteToken(JwtSecurityToken),
                UserId = NewAccount.Id,
                UserName = NewAccount.UserName,
                Password = user.Password
            };
        }

        public async Task<AuthModel> loginAsync(LoginDTO user)
        {
            var UserAccount = await _UserManager.FindByNameAsync(user.UserName);
            if (user is null || !await _UserManager.CheckPasswordAsync(UserAccount, user.Password))
            {
                return new AuthModel() { Message = "Email or Passwrod is incorrect!" };
            }
            JwtSecurityToken JwtSecurityToken;
            
            JwtSecurityToken = await CreateJwtToken(UserAccount);
            var RolesList = await _UserManager.GetRolesAsync(UserAccount);
            var role = RolesList.FirstOrDefault();
            return new AuthModel()
            {
                IsAuthenticated = true,
                Token = new JwtSecurityTokenHandler().WriteToken(JwtSecurityToken),
                UserId = UserAccount.Id,
                ExpireOn = JwtSecurityToken.ValidTo,
                Role = role
            };
        }

        public async Task<JwtSecurityToken> CreateJwtToken(User user, bool rememberme = false)
        {
            var userClaims = await _UserManager.GetClaimsAsync(user);
            var roles = await _UserManager.GetRolesAsync(user);
            var roleClaims = new List<Claim>();

            foreach (var role in roles)
                roleClaims.Add(new Claim("roles", role));

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim("uid", user.Id),
                new Claim("username",user.UserName)
            }
            .Union(userClaims)
            .Union(roleClaims);

            var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.Key));
            var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

            var jwtSecurityToken = new JwtSecurityToken(
            issuer: _jwt.ValidIssuer,
                audience: _jwt.ValidAudience,
                claims: claims,
                expires: (rememberme == true) ? DateTime.UtcNow.AddDays(_jwt.DurationInDays) : DateTime.UtcNow.AddDays(30),
                signingCredentials: signingCredentials);

            return jwtSecurityToken;
        }
        public async Task<AuthModel> registerationValidation(RegisterDTO user)
        {
            if (await _UserManager.FindByEmailAsync(user.Email) != null)
                return new AuthModel() { Message = "Email is already registered!" };
            if (user.PhoneNumber != null && _UserManager.Users.SingleOrDefault(ac => ac.PhoneNumber == user.PhoneNumber) != null)
                return new AuthModel() { Message = "This number is already Exist!" };
        
            return new AuthModel() { IsAuthenticated = true };
        }
    }
}
