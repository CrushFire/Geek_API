using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Core.Models.Community;

public class CommunityAddRequest
{
    public string Name { get; set; }

    public string Description { get; set; } = string.Empty;
    public string Categories {  get; set; }
    //аватарку сразу не грузим
}
