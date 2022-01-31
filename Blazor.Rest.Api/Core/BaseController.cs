using Blasor.Data.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;

namespace Blazor.Rest.Api.Core
{
    public class BaseController : ControllerBase
    {
        public Guid? AuthorizedUserId => ((User)HttpContext.Items["Account"])?.Id 
            ?? Guid.Parse(HttpContext.User.FindFirst("id")?.Value);
    }
}
