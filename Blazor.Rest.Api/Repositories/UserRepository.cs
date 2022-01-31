using Blasor.Data;
using Blasor.Data.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Blazor.Data.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly IdentityContext _context;

        public UserRepository(IdentityContext context)
        {
            _context = context;
        }

        public async Task<User> CreateAsync(string username, string email, string role = null)
        {
            var user = new User { Email = email, Name = username, Role = role };
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public Task<User> GetAsync(string username, string email)
        {
            return Task.FromResult( _context.Users
                .FirstOrDefault(u => u.Name.ToLower() ==  username.ToLower()
                    && u.Email.ToLower() == email.ToLower()));
            
        }

        public async Task<User> GetAsync(Guid? userId)
        {
            return await _context.Users
                .FindAsync(userId);
        }
    }
}
