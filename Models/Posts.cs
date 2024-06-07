namespace Blog.Models
{
    public class Posts
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public string? Image { get; set; }
        public string? UserId { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
