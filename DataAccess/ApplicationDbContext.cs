using DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace DataAccess;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions options) : base(options)
    {
    }

    public DbSet<EntityUser> Users { get; set; }
    public DbSet<EntityPost> Posts { get; set; }
    public DbSet<EntityCommunity> Communities { get; set; }
    public DbSet<EntityComment> Comments { get; set; }
    public DbSet<EntityCategory> Categories { get; set; }
    public DbSet<EntityImage> Images { get; set; }
    public DbSet<EntityUserCommunity> UsersCommunities { get; set; }
    public DbSet<EntityPostCategory> PostsCategories { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<EntityUser>()
            .HasMany(x => x.Posts)
            .WithOne(x => x.Author)
            .OnDelete(DeleteBehavior.NoAction);
        modelBuilder.Entity<EntityPost>()
            .HasOne(x => x.Author)
            .WithMany(x => x.Posts)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<EntityUser>()
            .HasMany(x => x.UserCommunities)
            .WithOne(x => x.User)
            .OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<EntityUserCommunity>()
            .HasOne(x => x.User)
            .WithMany(x => x.UserCommunities)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<EntityCommunity>()
            .HasMany(x => x.UserCommunities)
            .WithOne(x => x.Community)
            .OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<EntityUserCommunity>()
            .HasOne(x => x.Community)
            .WithMany(x => x.UserCommunities)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<EntityUser>()
            .HasMany(x => x.Comments)
            .WithOne(x => x.Author)
            .OnDelete(DeleteBehavior.NoAction);
        modelBuilder.Entity<EntityComment>()
            .HasOne(x => x.Author)
            .WithMany(x => x.Comments)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<EntityCommunity>()
            .HasMany(x => x.Posts)
            .WithOne(x => x.Community)
            .OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<EntityPost>()
            .HasOne(x => x.Community)
            .WithMany(x => x.Posts)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<EntityPost>()
            .HasMany(x => x.PostCategories)
            .WithOne(x => x.Post)
            .OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<EntityPostCategory>()
            .HasOne(x => x.Post)
            .WithMany(x => x.PostCategories)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<EntityCategory>()
            .HasMany(x => x.PostCategories)
            .WithOne(x => x.Category)
            .OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<EntityPostCategory>()
            .HasOne(x => x.Category)
            .WithMany(x => x.PostCategories)
            .OnDelete(DeleteBehavior.NoAction);
    }
}