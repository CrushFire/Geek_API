using Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace DataAccess;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions options) : base(options)
    {
    }

    public DbSet<Like> Likes { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Post> Posts { get; set; }
    public DbSet<Community> Communities { get; set; }
    public DbSet<Comment> Comments { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Image> Images { get; set; }
    public DbSet<UserCommunity> UsersCommunities { get; set; }
    public DbSet<PostCategory> PostsCategories { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<User>()
            .HasMany(x => x.Posts)
            .WithOne(x => x.Author)
            .OnDelete(DeleteBehavior.NoAction);
        modelBuilder.Entity<Post>()
            .HasOne(x => x.Author)
            .WithMany(x => x.Posts)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<User>()
            .HasMany(x => x.UserCommunities)
            .WithOne(x => x.User)
            .OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<UserCommunity>()
            .HasOne(x => x.User)
            .WithMany(x => x.UserCommunities)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<Community>()
            .HasMany(x => x.UserCommunities)
            .WithOne(x => x.Community)
            .OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<UserCommunity>()
            .HasOne(x => x.Community)
            .WithMany(x => x.UserCommunities)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<User>()
            .HasMany(x => x.Comments)
            .WithOne(x => x.Author)
            .OnDelete(DeleteBehavior.NoAction);
        modelBuilder.Entity<Comment>()
            .HasOne(x => x.Author)
            .WithMany(x => x.Comments)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<Community>()
            .HasMany(x => x.Posts)
            .WithOne(x => x.Community)
            .OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<Post>()
            .HasOne(x => x.Community)
            .WithMany(x => x.Posts)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<Post>()
            .HasMany(x => x.PostCategories)
            .WithOne(x => x.Post)
            .OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<PostCategory>()
            .HasOne(x => x.Post)
            .WithMany(x => x.PostCategories)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Category>()
            .HasMany(x => x.PostCategories)
            .WithOne(x => x.Category)
            .OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<PostCategory>()
            .HasOne(x => x.Category)
            .WithMany(x => x.PostCategories)
            .OnDelete(DeleteBehavior.NoAction);
    }
}