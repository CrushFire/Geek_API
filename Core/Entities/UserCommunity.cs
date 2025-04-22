using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Entities;

public class UserCommunity
{
    [Key] public long Id { get; set; }

    [Required] public long UserId { get; set; }

    [ForeignKey(nameof(UserId))] public User? User { get; set; } = null!;

    [Required] public long CommunityId { get; set; }

    [ForeignKey(nameof(CommunityId))] public Community? Community { get; set; } = null!;

    [Required] public string UserRole { get; set; } //owner, moderator, subscriber
}