using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Blog.Models;

namespace Blog.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<Posts> Posts { get; set; }
        public DbSet<Like> Likes { get; set; } // Adicionar DbSet para Like

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Posts>()
                .HasMany(p => p.Likes) // Um post pode ter muitos likes
                .WithOne(l => l.Post) // Um like pertence a um único post
                .HasForeignKey(l => l.PostId) // Chave estrangeira para o PostId em Like
                .OnDelete(DeleteBehavior.Cascade); // Se um post for excluído, exclua também todos os likes associados

            base.OnModelCreating(modelBuilder);
        }
    }
}
