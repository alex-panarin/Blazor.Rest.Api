using Blasor.Data.Models;
using Microsoft.EntityFrameworkCore.Storage;
using System.Threading.Tasks;

namespace Blazor.Data.Repositories
{
    public interface ITokenRepository
    {
        Task CreateAsync(User user, string password);
        Task<Token> VerifyAsync(User user, string password);
        Task<Token> RefreshAsync(User user, string token);
        Task<Token> GetAsync(string userId);

        Task<IDbContextTransaction> GetTransactionAsync();
    }
}