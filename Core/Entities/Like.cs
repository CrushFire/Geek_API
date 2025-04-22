using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Entities;

public class Like
{
    [Key] public long Id { get; set; }

    [Required] public long UserId { get; set; }

    [ForeignKey(nameof(UserId))] public User? User { get; set; }

    [Required] public long PostId { get; set; }

    [ForeignKey(nameof(PostId))] public Post? Post { get; set; }

    public bool IsLike { get; set; } = true; // true = Like, false = Dislike

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}