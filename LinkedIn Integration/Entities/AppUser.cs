using Microsoft.AspNetCore.Identity;

namespace LinkedIn_Integration.Entities
{
    public class AppUser : IdentityUser
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public string ExpiresIn { get; set; }
       // public bool IsAuthenticated { get; set; }
    }
}
