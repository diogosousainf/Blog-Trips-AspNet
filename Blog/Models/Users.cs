using Microsoft.AspNetCore.Identity;

namespace Blog.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string Name{ get; set; }
        public string Email { get; set; }
        public string Role { get; set; }

    }
}
