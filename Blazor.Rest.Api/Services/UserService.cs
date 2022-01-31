using Blazor.Data.Repositories;
using Blazor.Identity.Api.Controllers.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Threading.Tasks;

namespace Blazor.Identity.Api.Services
{
    internal class UserService : IUserService
    {
        private readonly ITokenRepository _tokenRepository;
        private readonly IUserRepository _userRepository;

        public UserService(ITokenRepository tokenRepository,
            IUserRepository userRepository)
        {
            _tokenRepository = tokenRepository;
            _userRepository = userRepository;
        }

        public async Task CreateAsync(string username, string email, string password, string role = null)
        {
            IDbContextTransaction transaction = null;
            try
            {
                using (var trans = await _tokenRepository.GetTransactionAsync())
                {
                    transaction = trans;
                    var user = await _userRepository.GetAsync(username, email);
                    if (user != null)
                        throw new Exception($"User: '{username}' already created");

                    user = await _userRepository.CreateAsync(username, email, role);
                    await _tokenRepository.CreateAsync(user, password);
                    await trans.CommitAsync();
                }
            }
            catch (DbUpdateException)
            {
                await transaction.RollbackAsync();
                throw;
            }
            
        }
        public async Task<TokenResponse> LoginAsync(string username, string email, string password, string role = null)
        {
            var user = await _userRepository.GetAsync(username, email);
            if (user == null)
                throw new Exception($"User: '{username}' not found");

            var token = await _tokenRepository.VerifyAsync(user, password);
            return new TokenResponse
            {
                Token = token.AccessToken,
                RefreshToken = token.RefreshToken
            };
        }

        public async Task<TokenResponse> RefreshAsync(Guid? userId, string refresh)
        {
            IDbContextTransaction transaction = null;
            try
            {
                using (var trans = await _tokenRepository.GetTransactionAsync())
                {
                    transaction = trans;
                    var user = await _userRepository.GetAsync(userId);
                    if (user == null)
                        throw new Exception($"User not found");

                    var token = await _tokenRepository.RefreshAsync(user, refresh);
                    
                    await trans.CommitAsync();
                    
                    return new TokenResponse
                    {
                        Token = token.AccessToken,
                        RefreshToken = token.RefreshToken
                    };
                }
            }
            catch (DbUpdateException)
            {
                await transaction.RollbackAsync();
                throw;
            }

        }
    }
}
