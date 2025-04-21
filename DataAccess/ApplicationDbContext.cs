using DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace DataAccess;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions options) : base(options)
    {
    }

    public DbSet<UserEntity> Users { get; set; }
    public DbSet<PostEntity> Posts { get; set; }
    public DbSet<CommunityEntity> Communities { get; set; }
    public DbSet<CommentEntity> Comments { get; set; }
    public DbSet<CategoryEntity> Categories { get; set; }
    public DbSet<ImageEntity> Images { get; set; }
    public DbSet<UserCommunityEntity> UsersCommunities { get; set; }
    public DbSet<PostCategoryEntity> PostsCategories { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<UserEntity>()
            .HasMany(x => x.Posts)
            .WithOne(x => x.Author)
            .OnDelete(DeleteBehavior.NoAction);
        modelBuilder.Entity<PostEntity>()
            .HasOne(x => x.Author)
            .WithMany(x => x.Posts)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<UserEntity>()
            .HasMany(x => x.UserCommunities)
            .WithOne(x => x.User)
            .OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<UserCommunityEntity>()
            .HasOne(x => x.User)
            .WithMany(x => x.UserCommunities)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<CommunityEntity>()
            .HasMany(x => x.UserCommunities)
            .WithOne(x => x.Community)
            .OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<UserCommunityEntity>()
            .HasOne(x => x.Community)
            .WithMany(x => x.UserCommunities)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<UserEntity>()
            .HasMany(x => x.Comments)
            .WithOne(x => x.Author)
            .OnDelete(DeleteBehavior.NoAction);
        modelBuilder.Entity<CommentEntity>()
            .HasOne(x => x.Author)
            .WithMany(x => x.Comments)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<CommunityEntity>()
            .HasMany(x => x.Posts)
            .WithOne(x => x.Community)
            .OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<PostEntity>()
            .HasOne(x => x.Community)
            .WithMany(x => x.Posts)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<PostEntity>()
            .HasMany(x => x.PostCategories)
            .WithOne(x => x.Post)
            .OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<PostCategoryEntity>()
            .HasOne(x => x.Post)
            .WithMany(x => x.PostCategories)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<CategoryEntity>()
            .HasMany(x => x.PostCategories)
            .WithOne(x => x.Category)
            .OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<PostCategoryEntity>()
            .HasOne(x => x.Category)
            .WithMany(x => x.PostCategories)
            .OnDelete(DeleteBehavior.NoAction);
    }
}