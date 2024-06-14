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
        // Propriedades para likes
        public int LikeCount { get; set; } // Contagem de likes

        // Relacionamento com likes
        public ICollection<Like> Likes { get; set; } = new List<Like>(); // Inicialize a coleção

    }
}

