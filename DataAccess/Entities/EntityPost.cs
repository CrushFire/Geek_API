using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccess.Entities;

public class EntityPost
{
    [Key] public long Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Content { get; set; } = string.Empty;

    [Required] public long AuthorId { get; set; }

    [ForeignKey(nameof(AuthorId))] public EntityUser? Author { get; set; } = null;

    [Required] public long CommunityId { get; set; }

    [ForeignKey(nameof(CommunityId))] public EntityCommunity? Community { get; set; } = null;

    public List<EntityImage> Images { get; set; } = new();

    public int Views { get; set; } = 0;

    public int Likes { get; set; } = 0;

    public int Dislikes { get; set; } = 0;

    public List<EntityPostCategory> PostCategories { get; set; } = new();

    public DateTime CreateAt { get; set; } = DateTime.UtcNow;
}