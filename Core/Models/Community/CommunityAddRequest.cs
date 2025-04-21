using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Core.Models.Community;

public class CommunityAddRequest
{
    public string Name { get; set; }

    public string Description { get; set; } = string.Empty;

    public IFormFile? Image { get; set; }

    public List<int> CategoriesId { get; set; } = new();
}
