using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccess.Entities;

public class EntityComment
{
    [Key] public long Id { get; set; }

    [Required] public string Content { get; set; };

    [Required] public long AuthorId { get; set; }

    [ForeignKey(nameof(AuthorId))] public EntityUser? Author { get; set; } = null;

    [Required] public long PostId { get; set; }

    [ForeignKey(nameof(PostId))] public EntityPost? Post { get; set; } = null;

    public DateTime CreateAt { get; set; } = DateTime.UtcNow;
}