using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccess.Entities;

public class PostEntity
{
    [Key] public long Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Content { get; set; } = string.Empty;

    [Required] public long AuthorId { get; set; }

    [ForeignKey(nameof(AuthorId))] public UserEntity? Author { get; set; } = null;

    [Required] public long CommunityId { get; set; }

    [ForeignKey(nameof(CommunityId))] public CommunityEntity? Community { get; set; } = null;

    public List<ImageEntity> Images { get; set; } = new();

    public int Views { get; set; } = 0;

    public int Likes { get; set; } = 0;

    public int Dislikes { get; set; } = 0;

    public List<PostCategoryEntity> PostCategories { get; set; } = new();

    public DateTime CreateAt { get; set; } = DateTime.UtcNow;
}