using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Entities;

public class Post
{
    [Key] public long Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Content { get; set; } = string.Empty;

    [Required] public long AuthorId { get; set; }

    [ForeignKey(nameof(AuthorId))] public User? Author { get; set; } = null;

    public long? CommunityId { get; set; } //тк юзер может от своего имени закреейтить

    [ForeignKey(nameof(CommunityId))] public Community? Community { get; set; } = null;

    public List<Image> Images { get; set; } = new();

    public int Views { get; set; } = 0;
    [Required]
    public string Categories { get; set; } = string.Empty;
    public List<Comment> Comments { get; set; } = new();
    public List<Like> Reactions { get; set; } = new();

    public DateTime CreateAt { get; set; } = DateTime.UtcNow;
}