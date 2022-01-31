using Blazor.Identity.Api.Controllers.Models;
using Blazor.Identity.Api.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace Blazor.Identity.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class LoginController : ControllerBase
    {
        private readonly IUserService _userService;

        public LoginController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost]
        public async Task<ActionResult<TokenResponse>> Post([FromBody]LoginRequest request)
        {
            try
            {
                var response = await _userService.LoginAsync(request.name, request.email, request.password, request.role);
                
                if (response == null)
                    return Unauthorized("Invalid credentials");

                return Ok(response);
            }
            catch(Exception x)
            {
                return BadRequest(x.Message);
            }

        }
    }
}
