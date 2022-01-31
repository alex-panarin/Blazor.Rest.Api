namespace Blazor.Identity.Api.Controllers.Models
{
    public class LoginRequest
    {
        public string role { get; set; }
        public string password { get; set; }
        public string email { get; set; }
        public string name { get; set; }
    }
}
