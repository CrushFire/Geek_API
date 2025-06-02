using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Core.Models.Community;

public class CommunityAddRequest
{
    public string Name { get; set; }

    public string Description { get; set; } = string.Empty;
    public List<int> CategoriesIds {  get; set; }
    public IFormFile Avatar { get; set; }
    //аватарку сразу не грузим
}
