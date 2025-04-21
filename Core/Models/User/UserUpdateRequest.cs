using System.ComponentModel.DataAnnotations;

namespace Core.Models;

public class UserUpdateRequest
{
    public string UserName { get; set; }

    public string? Description { get; set; } = string.Empty;
    public string? AvatarUrl { get; set; } //ссылка по умолчанию на картинку
    public List<ImageResponse> Images { get; set; } = new();
}