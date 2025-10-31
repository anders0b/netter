using Microsoft.EntityFrameworkCore;
using Netter.Domain.Users;
using Netter.Domain.Posts;
using Netter.Domain.SocialInteractions;

namespace Netter.Infrastructure.Persistence;

public class NetterDbContext(DbContextOptions<NetterDbContext> options) : DbContext(options)
{
    public DbSet<User> Users { get; set; }
    public DbSet<Post> Posts { get; set; }
    public DbSet<Follow> Follows { get; set; }
    public DbSet<Like> Likes { get; set; }
    public DbSet<Comment> Comments { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply configurations
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(NetterDbContext).Assembly);
    }
}