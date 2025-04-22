using Core.Models.Category;

namespace Core.Models.Community;

public class CommunityResponse
{
    public long Id { get; set; }

    public string Name { get; set; }

    public string Description { get; set; } = string.Empty;

    public List<ImageResponse> Images { get; set; } = new();

    //ublic List<CategoryResponse> Categories { get; set; } = new();

    //TODO
    //Кол-во постов? Опять таки лишний join
}