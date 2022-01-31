using System;

namespace Blasor.Data.Models
{
    public class Token
    {
        public Guid TokenId { get; set; } = Guid.NewGuid();
        public string UserId { get; set; } 
        public string Hash { get; set; }
        public string Salt { get; set; }
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
    }
}
