namespace Core.Models;

public class UserUpdateRequest
{
    public string UserName { get; set; }

    public string? Description { get; set; } = string.Empty;
}