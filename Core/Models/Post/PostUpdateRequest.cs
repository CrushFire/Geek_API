using Microsoft.AspNetCore.Http;

namespace Core.Models.Post;

public class PostUpdateRequest
{
    public string Title { get; set; } = string.Empty;

    public string Content { get; set; } = string.Empty;
    public int CommunityId { get; set; }
    public List<int> Categories { get; set; } = new();
    public List<long> ImagesToRemove { get; set; } = new();
    public List<IFormFile> NewImages { get; set; } = new();
}