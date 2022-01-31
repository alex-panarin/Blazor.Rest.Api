using Blasor.Data.Models;
using System;
using System.Threading.Tasks;

namespace Blazor.Data.Repositories
{
    public interface IUserRepository
    {
        Task<User> GetAsync(string username, string email);
        Task<User> CreateAsync(string username, string email, string role);
        Task<User> GetAsync(Guid? userId);
    }
}
