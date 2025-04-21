using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Core.Models.Image;

namespace Core.Models.Post;
public class PostAddRequest
{
    public string Title { get; set; } = string.Empty;

    public string Content { get; set; } = string.Empty;

    public long CommunityId { get; set; }

    public List<ImageAddRequest> Images { get; set; } = new();
}
