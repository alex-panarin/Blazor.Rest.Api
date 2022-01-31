using Blasor.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace Blazor.Data
{
    public class IdentityContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Token> Tokens { get; set; }

        public IdentityContext(DbContextOptions<IdentityContext> options)
            : base(options)
        {
            
        }
    }
}
