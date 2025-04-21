using System.ComponentModel.DataAnnotations;

namespace DataAccess.Entities;

public class EntityCommunity
{
    [Required] public long Id { get; set; }

    [Required] public string Name { get; set; }

    public string? Description { get; set; }

    public List<EntityImage> Images { get; set; } = new();

    public List<EntityCategory> Categories { get; set; } = new();

    public List<EntityUserCommunity> UserCommunities { get; set; } = new();
    public List<EntityPost> Posts { get; set; } = new();

    public DateTime CreateAt { get; set; } = DateTime.UtcNow;
}