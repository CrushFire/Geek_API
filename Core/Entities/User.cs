using System.ComponentModel.DataAnnotations;

namespace Core.Entities;

public class User
{
    [Key] public long Id { get; set; }

    [Required] public string UserName { get; set; }

    [Required] public string PasswordHash { get; set; }

    public string? Description { get; set; } = string.Empty;

    public List<Image> Images { get; set; } = new();

    public List<Post> Posts { get; set; } = new();
    public List<UserCommunity> UserCommunities { get; set; } = new();
    public List<Comment> Comments { get; set; } = new();
    [Required] public string Role { get; set; }

    public DateTime CreateAt { get; set; } = DateTime.UtcNow;
}