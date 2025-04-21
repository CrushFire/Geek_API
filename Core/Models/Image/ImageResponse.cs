using System.ComponentModel.DataAnnotations;

namespace Core.Models;

public class ImageResponse
{
    public long Id { get; set; } //для дропа

    public string ImageUrl { get; set; } = string.Empty;

    public string ImageType { get; set; } //?????
}