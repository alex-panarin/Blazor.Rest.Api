using Blazor.Identity.Api.Controllers.Models;
using System;
using System.Threading.Tasks;

namespace Blazor.Identity.Api.Services
{
    public interface IUserService
    {
        Task<TokenResponse> LoginAsync(string username, string email, string password, string role);
        Task CreateAsync(string username, string email, string password, string role);
        Task<TokenResponse> RefreshAsync(Guid? userId, string refresh);
    }
}
