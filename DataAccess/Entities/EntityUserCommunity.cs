using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccess.Entities;

public class EntityUserCommunity
{
    [Key] public long Id { get; set; }

    [Required] public long UserId { get; set; }

    [ForeignKey(nameof(UserId))] public EntityUser? User { get; set; } = null!;

    [Required] public long CommunityId { get; set; }

    [ForeignKey(nameof(CommunityId))] public EntityCommunity? Community { get; set; } = null!;

    [Required] public string UserRole { get; set; } //owner, moderator, subscriber
}