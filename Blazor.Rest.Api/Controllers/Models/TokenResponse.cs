﻿namespace Blazor.Identity.Api.Controllers.Models 
{
    
    public class TokenResponse
    {
        public string Token { get; set; }
        public string RefreshToken { get; set; }
    }
}
