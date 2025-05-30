using Core.Models.Category;

namespace Core.Models.Community;

public class CommunityResponse
{
    public long Id { get; set; }

    public string CommunityName { get; set; }

    public string? Description { get; set; } = string.Empty;
    public string AvatarUrl { get; set; }
    public List<string> CategoriesRu {  get; set; }
    public List<string> CategoriesEng { get; set; }
    public int NumberOfMember { get; set; }
    public UserResponse Author {  get; set; }
    public DateTime CreateAt { get; set; }
}