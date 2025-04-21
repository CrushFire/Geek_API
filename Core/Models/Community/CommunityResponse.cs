using Core.Models.Category;
using System.ComponentModel.DataAnnotations;

namespace Core.Models.Community;

public class CommunityResponse
{
    public long Id { get; set; }

    public string Name { get; set; }

    public string Description { get; set; } = string.Empty;

    public List<ImageResponse> Images { get; set; } = new();

    public List<CategoryResponse> Categories { get; set; } = new();

}
