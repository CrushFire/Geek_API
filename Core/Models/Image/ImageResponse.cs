using System.ComponentModel.DataAnnotations;

namespace Core.Models;

public class ImageResponse
{
    [Required] public int EntityId;

    [Required] public int Id { get; set; }

    [Required] public string Title { get; set; } = string.Empty;

    [Required] public string Alt { get; set; } = string.Empty;

    public string? Description { get; set; } = string.Empty;

    [Required] public string ImageUrl { get; set; } = string.Empty;

    [Required] public DateTime CreateAt { get; } = DateTime.UtcNow;

    [Required] public string EntityTarget { get; set; } //Либо Enum
}