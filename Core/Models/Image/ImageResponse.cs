namespace Core.Models;

public class ImageResponse
{
    public long Id { get; set; }

    public string ImageUrl { get; set; } = string.Empty;

    public string ImageType { get; set; } //Нужно ли?
}