using System.ComponentModel.DataAnnotations;

namespace DataAccess.Entities;

public class EntityUser
{
    [Key] public long Id { get; set; }

    [Required] public string UserName { get; set; }

    [Required] public string PasswordHash { get; set; }

    public string? Description { get; set; } = string.Empty;

    public List<EntityImage> Images { get; set; } = new();

    public List<EntityPost> Posts { get; set; } = new();
    public List<EntityUserCommunity> UserCommunities { get; set; } = new();
    public List<EntityComment> Comments { get; set; } = new();

    public DateTime CreateAt { get; set; } = DateTime.UtcNow;
}