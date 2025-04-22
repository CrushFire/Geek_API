using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Entities;

public class Comment
{
    [Key] public long Id { get; set; }

    [Required] public string Content { get; set; }

    [Required] public long AuthorId { get; set; }

    [ForeignKey(nameof(AuthorId))] public User? Author { get; set; } = null;

    [Required] public long PostId { get; set; }

    [ForeignKey(nameof(PostId))] public Post? Post { get; set; } = null;

    public DateTime CreateAt { get; set; } = DateTime.UtcNow;
}