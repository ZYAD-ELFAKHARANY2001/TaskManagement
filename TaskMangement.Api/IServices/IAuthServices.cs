using TaskMangement.Core.Models;
using TaskMangement.Core.DTOs;



namespace TaskMangement.Api.IServices
{
    public interface IAuthServices
    {
        Task<AuthModel> RegisterAsync(RegisterDTO user);
        Task<AuthModel> loginAsync(LoginDTO user);
    }
}
