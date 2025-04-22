using System.ComponentModel.DataAnnotations;

namespace Core.Entities;

public class Community
{
    [Key] public long Id { get; set; }

    [Required] public string Name { get; set; }

    public string? Description { get; set; }

    public List<Image> Images { get; set; } = new();

    public List<Category> Categories { get; set; } = new();

    public List<UserCommunity> UserCommunities { get; set; } = new();
    public List<Post> Posts { get; set; } = new();

    public DateTime CreateAt { get; set; } = DateTime.UtcNow;
}