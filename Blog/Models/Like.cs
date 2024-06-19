using Microsoft.AspNetCore.Identity;

namespace Blog.Models
{
    public class Like
    {
        public int Id { get; set; }
        public int PostId { get; set; }
        public string UserId { get; set; }

        // Relacionamento com posts
        public Posts Post { get; set; }

        // Relacionamento com users
        public IdentityUser User { get; set; }
    }
}
