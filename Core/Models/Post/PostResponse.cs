using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Core.Models.Community;

namespace Core.Models.Post;
public class PostResponse
{
    public long Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Content { get; set; } = string.Empty;

    public UserResponse Author { get; set; }

    public long CommunityId { get; set; }

    public List<ImageResponse> Images { get; set; } = new();

    public int Views { get; set; } = 0;

    public int Likes { get; set; } = 0;

    public int Dislikes { get; set; } = 0;

    public DateTime CreateAt { get; set; }
}
