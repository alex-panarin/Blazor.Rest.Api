using Blasor.Data;
using Blasor.Data.Models;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Blazor.Data.Repositories
{
    public class TokenRepository : ITokenRepository
    {
        private readonly IdentityContext _context;
        private readonly IConfiguration _configuration;

        public TokenRepository(IdentityContext context, 
            IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }
        private string _Key => _configuration.GetValue<string>("SecretKey");
        public async Task<IDbContextTransaction> GetTransactionAsync()
        {
            return await _context.Database.BeginTransactionAsync();
        }
        public Task<Token> GetAsync(string userId)
        {
            return Task.FromResult( _context.Tokens
                .FirstOrDefault(t => t.UserId == userId));   
        }
        public async Task CreateAsync(User user, string password)
        {
            CreatePasswordHash(password, out byte[] hash, out byte[] salt);
            var createdToken = new Token
            {
                AccessToken = CreateToken(_Key, user.Id.ToString(), user.Name, user.Email, user.Role),
                Hash = Convert.ToBase64String(hash),
                Salt = Convert.ToBase64String(salt),
                UserId = user.Id.ToString()
            };
            
            createdToken.CreateRefresh();

            _context.Tokens.Add(createdToken);
            await _context.SaveChangesAsync();
        }

        public async Task<Token> RefreshAsync(User user, string refreshToken)
        {
            var savedToken = await GetAsync(user.Id.ToString());
            if (savedToken == null)
                throw new Exception("User not registered");

            if(savedToken.RefreshToken != refreshToken)
                throw new Exception("Bad token");

            var createdToken = new Token
            {
                AccessToken = CreateToken(_Key, user.Id.ToString(), user.Name, user.Email, user.Role),
                Hash = savedToken.Hash,
                Salt = savedToken.Salt,
                UserId = user.Id.ToString(),
            };

            createdToken.CreateRefresh();

            _context.Tokens.Add(createdToken);
            _context.Tokens.Remove(savedToken);
            await _context.SaveChangesAsync();

            return createdToken;
        }

        public async Task<Token> VerifyAsync(User user, string password)
        {
            var savedToken = await GetAsync(user.Id.ToString());
            
            if (!VerifyPasswordHash(password, Convert.FromBase64String(savedToken.Hash), Convert.FromBase64String(savedToken.Salt)))
                throw new Exception("Bad credentials");

            return savedToken;
         }
        private string CreateToken(string tokenKey, string id, string name, string email, string role = null)
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, name),
                new Claim(ClaimTypes.Email, email),
                new Claim("id", id)
            };
            if (role != null)
                claims.Add(new Claim(ClaimTypes.Role, role));

            var tokenHandler = new JwtSecurityTokenHandler();

            var credentials = new SigningCredentials(
                new SymmetricSecurityKey(Encoding.ASCII.GetBytes(tokenKey)),
                SecurityAlgorithms.HmacSha512Signature);

            var desctiptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = credentials
            };
            var token = tokenHandler.CreateJwtSecurityToken(desctiptor);
            var jwt = tokenHandler.WriteToken(token);

            return jwt;
        }

        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            }
        }

        private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512(passwordSalt))
            {
                var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
                return computedHash.SequenceEqual(passwordHash);
            }
        }
    }

    static class TokenExtensions
    {
        public static void CreateRefresh(this Token token)
        {
            using (var hmac = new HMACSHA512(Encoding.UTF8.GetBytes(token.Salt)))
            {
                token.RefreshToken = Convert.ToBase64String(hmac.ComputeHash(Encoding.UTF8.GetBytes(token.TokenId.ToString())));
            }
        }
    }
}
