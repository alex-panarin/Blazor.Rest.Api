using Blazor.Identity.Api.Controllers.Models;
using Blazor.Identity.Api.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace Blazor.Identity.Api.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class CreateController : ControllerBase
    {
        private readonly IUserService _userService;

        public CreateController(IUserService  userService)
        {
            _userService = userService;
        }

        [HttpPost]
        public async Task<ActionResult> Create([FromBody] CreateRequest request)
        {
            try
            {
                await _userService.CreateAsync(request.Name, request.Email, request.Password, request.Role);
                return Ok();
            }
            catch (Exception x)
            {
                return BadRequest(x.Message);
            }
        }
    }
}
