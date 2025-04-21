using System.ComponentModel.DataAnnotations;

namespace Core.Models;

public class UserRequest
{
    [Required] public string UserName { get; set; }

    [Required] public string Password { get; set; }

    public string? Description { get; set; } = string.Empty;
    public string? AvatarUrl { get; set; } = null; //ссылка по умолчанию на картинку
}