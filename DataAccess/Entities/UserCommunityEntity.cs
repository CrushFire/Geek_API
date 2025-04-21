using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccess.Entities;

public class UserCommunityEntity
{
    [Key] public long Id { get; set; }

    [Required] public long UserId { get; set; }

    [ForeignKey(nameof(UserId))] public UserEntity? User { get; set; } = null!;

    [Required] public long CommunityId { get; set; }

    [ForeignKey(nameof(CommunityId))] public CommunityEntity? Community { get; set; } = null!;

    [Required] public string UserRole { get; set; } //owner, moderator, subscriber
}