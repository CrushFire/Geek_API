using System.ComponentModel.DataAnnotations;

namespace DataAccess.Entities;

public class UserEntity
{
    [Key] public long Id { get; set; }

    [Required] public string UserName { get; set; }

    [Required] public string PasswordHash { get; set; }

    public string? Description { get; set; } = string.Empty;

    public List<ImageEntity> Images { get; set; } = new();

    public List<PostEntity> Posts { get; set; } = new();
    public List<UserCommunityEntity> UserCommunities { get; set; } = new();
    public List<CommentEntity> Comments { get; set; } = new();
    [Required] public string Role { get; set; }

    public DateTime CreateAt { get; set; } = DateTime.UtcNow;
}