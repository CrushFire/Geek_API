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
    public DbSet<PostCategory> PostCategories { get; set; }
    public DbSet<Community> Communities { get; set; }
    public DbSet<CommunityCategory> CommunityCategories { get; set; }
    public DbSet<Comment> Comments { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Image> Images { get; set; }
    public DbSet<UserCommunity> UsersCommunities { get; set; }
    public DbSet<DataPage> DataPages { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        ////////////УДАЛИ В БД ВСЕ ВНЕШНИЕ КЛЮЧИ У IMAGE\\\\\\\\\\\\\\\\\\\\\
        modelBuilder.Entity<Image>(entity =>
        {
            // Связь с User, если EntityTarget = "User"
            entity.HasOne<User>()
                .WithMany(u => u.Images) // Навигационное свойство для User
                .HasForeignKey(i => i.EntityId) // Связь через EntityId
                .OnDelete(DeleteBehavior.Cascade);

            // Связь с Community, если EntityTarget = "Community"
            entity.HasOne<Community>()
                .WithMany(c => c.Images) // Навигационное свойство для Community
                .HasForeignKey(i => i.EntityId) // Связь через EntityId
                .OnDelete(DeleteBehavior.Cascade);

            // Связь с Post, если EntityTarget = "Post"
            entity.HasOne<Post>()
                .WithMany(p => p.Images) // Навигационное свойство для Post
                .HasForeignKey(i => i.EntityId) // Связь через EntityId
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Настройка сущности User
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasMany(u => u.Images)
                .WithOne() // Навигационное свойство для Image
                .HasForeignKey(i => i.EntityId)
                .HasConstraintName("FK_User_Images");

            entity.HasMany(u => u.Posts)
                .WithOne(p => p.Author) // Навигационное свойство для Post
                .HasForeignKey(p => p.AuthorId)
                .HasConstraintName("FK_User_Posts");
        });

        modelBuilder.Entity<Community>(entity =>
        {
            entity.HasMany(c => c.Images)
                .WithOne() // Навигационное свойство для Image
                .HasForeignKey(i => i.EntityId)
                .HasConstraintName("FK_Community_Images");

            entity.HasMany(c => c.UserCommunities)
                .WithOne(uc => uc.Community) // Навигационное свойство для UserCommunity
                .HasForeignKey(uc => uc.CommunityId)
                .HasConstraintName("FK_Community_UserCommunities");

            entity.HasMany(c => c.Posts)
                .WithOne(p => p.Community) // Навигационное свойство для Post
                .HasForeignKey(p => p.CommunityId)
                .HasConstraintName("FK_Community_Posts");
        });

        modelBuilder.Entity<Post>(entity =>
        {
            entity.HasMany(p => p.Images)
                .WithOne() // Навигационное свойство для Image
                .HasForeignKey(i => i.EntityId)
                .HasConstraintName("FK_Post_Images");
        });

        modelBuilder.Entity<Like>(entity =>
        {
            entity.HasOne(l => l.User)
                .WithMany(l => l.Reactions) // Навигационное свойство для User
                .HasForeignKey(l => l.UserId)
                .HasConstraintName("FK_Like_User")
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(l => l.Post)
                .WithMany(l => l.Reactions) // Навигационное свойство для Post
                .HasForeignKey(l => l.PostId)
                .HasConstraintName("FK_Like_Post")
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Comment>(entity =>
        {
            entity.HasOne(c => c.Author)
                .WithMany(u => u.Comments) // Навигационное свойство для User
                .HasForeignKey(c => c.AuthorId)
                .HasConstraintName("FK_Comment_Author")
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(c => c.Post)
                .WithMany(c => c.Comments) // Навигационное свойство для Post
                .HasForeignKey(c => c.PostId)
                .HasConstraintName("FK_Comment_Post")
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<UserCommunity>(entity =>
        {
            entity.HasOne(uc => uc.User)
                .WithMany(u => u.UserCommunities) // Навигационное свойство для User
                .HasForeignKey(uc => uc.UserId)
                .HasConstraintName("FK_UserCommunity_User")
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(uc => uc.Community)
                .WithMany(c => c.UserCommunities) // Навигационное свойство для Community
                .HasForeignKey(uc => uc.CommunityId)
                .HasConstraintName("FK_UserCommunity_Community")
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<PostCategory>(entity =>
        {
            entity.HasOne(pc => pc.Post)
                .WithMany(p => p.PostCategories) // Навигационное свойство для Post
                .HasForeignKey(pc => pc.PostId)
                .HasConstraintName("FK_PostCategory_Post")
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(pc => pc.Category)
                .WithMany(c => c.PostCategories) // Навигационное свойство для Category
                .HasForeignKey(pc => pc.CategoryId)
                .HasConstraintName("FK_PostCategory_Category")
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<CommunityCategory>(entity =>
        {
            entity.HasOne(cc => cc.Community)
                .WithMany(c => c.CommunityCategories) // Навигационное свойство для Post
                .HasForeignKey(cc => cc.CommunityId)
                .HasConstraintName("FK_CommunityCategory_Community")
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(cc => cc.Category)
                .WithMany(c => c.CommunityCategories) // Навигационное свойство для Category
                .HasForeignKey(cc => cc.CategoryId)
                .HasConstraintName("FK_CommunityCategory_Category")
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}