using Blazor.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blazor.Rest.Api.Controllers.Jwt
{
    public class JwtMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IConfiguration _configuration;

        public JwtMiddleware(RequestDelegate next,
            IConfiguration configuration)
        {
            _next = next;
            _configuration = configuration;
        }

        public async Task Invoke(HttpContext context, IdentityContext identity)
        {

            var token = context.Request.Headers["Authorization"]
                .FirstOrDefault()?
                .Split(" ")
                .Last();

            if (token != null)
                await AttachAccountToContext(context, identity, token);

            await _next(context);
        }

        private async Task AttachAccountToContext(HttpContext context, IdentityContext identity, string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var read =  tokenHandler.ReadJwtToken(token);
                if (read == null)
                    throw new Exception("Invalid token");
                
                var key = Encoding.ASCII.GetBytes(_configuration.GetValue<string>("SecretKey"));
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    // set clockskew to zero so tokens expire exactly at token expiration time (instead of 5 minutes later)
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                var jwtToken = (JwtSecurityToken)validatedToken;
                var accountId = Guid.Parse(jwtToken.Claims.First(x => x.Type == "id").Value);
                // attach account to context on successful jwt validation
                context.Items["Account"] = await identity.Users.FindAsync(accountId);
            }
            catch
            {
                // do nothing if jwt validation fails
                // account is not attached to context so request won't have access to secure routes
            }
        }
    }
}
