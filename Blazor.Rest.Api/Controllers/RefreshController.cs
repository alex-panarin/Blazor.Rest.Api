using Blazor.Identity.Api.Controllers.Models;
using Blazor.Identity.Api.Services;
using Blazor.Rest.Api.Core;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace Blazor.Identity.Api.Controllers
{
    [Route("[controller]")]
    [ApiController]
    //[JwtAuthorize]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class RefreshController : BaseController
    {
        private readonly IUserService _userService;

        public RefreshController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost]
        public async Task<ActionResult<TokenResponse>> Post([FromBody] RefreshRequest request)
        {
            try
            {
                var userId = AuthorizedUserId;
                if(userId == null)
                    return Unauthorized("User not authorized");

                var token = await _userService.RefreshAsync(userId, request.refreshToken);
                
                return Ok(token);
            }
            catch(Exception x)
            {
                return BadRequest(x.Message);
            }
        }
    }
}
