namespace Blog.Models
{
    public class Posts
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public string? Image { get; set; }
        public string? UserId { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? Email { get; set; }
        public int LikeCount { get; set; } 

        // Relacionamento 
        public ICollection<Like> Likes { get; set; } = new List<Like>(); 

    }
}

