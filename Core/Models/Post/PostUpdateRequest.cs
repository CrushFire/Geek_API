using Microsoft.AspNetCore.Http;

namespace Core.Models.Post;

public class PostUpdateRequest
{
    public string Title { get; set; } = string.Empty;

    public string Content { get; set; } = string.Empty;
    public string Categories { get; set; }

    public List<IFormFile> Images { get; set; } = new();
}