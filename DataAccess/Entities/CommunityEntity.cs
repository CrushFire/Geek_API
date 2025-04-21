using System.ComponentModel.DataAnnotations;

namespace DataAccess.Entities;

public class CommunityEntity
{
    [Required] public long Id { get; set; }

    [Required] public string Name { get; set; }

    public string? Description { get; set; }

    public List<ImageEntity> Images { get; set; } = new();

    public List<CategoryEntity> Categories { get; set; } = new();

    public List<UserCommunityEntity> UserCommunities { get; set; } = new();
    public List<PostEntity> Posts { get; set; } = new();

    public DateTime CreateAt { get; set; } = DateTime.UtcNow;
}