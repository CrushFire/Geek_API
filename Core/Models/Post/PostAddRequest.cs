using Microsoft.AspNetCore.Http;

namespace Core.Models.Post;

public class PostAddRequest
{
    public string Title { get; set; } = string.Empty;

    public string Content { get; set; } = string.Empty;

    public long CommunityId { get; set; }

    public string Categories { get; set; }

    public List<IFormFile> Images { get; set; } = new();
}