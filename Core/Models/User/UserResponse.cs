using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Models;

public class UserResponse
{
    [Required] public int Id { get; set; }

    [Required] public string UserName { get; set; }

    public string? Description { get; set; } = string.Empty;
    public string? AvatarUrl { get; set; } = null;

    [NotMapped] public List<ImageResponse>? Banners { get; set; } = null;

    [Required] public int NumberOfPosts { get; set; } = 0;

    [Required] public int NumberOfComments { get; set; } = 0;
}