using System.ComponentModel.DataAnnotations;

namespace DataAccess.Entities;

public class ImageEntity
{
    [Key] public long Id { get; set; }

    [Required] public string ImageUrl { get; set; } = string.Empty;

    [Required] public string ImageType { get; set; } //аватарка, баннер и т.д. и т.п.
    [Required] public DateTime CreateAt { get; } = DateTime.UtcNow;

    [Required] public string EntityTarget { get; set; }

    [Required] public long EntityId { get; set; }
}