using Core.Entities;
using Microsoft.AspNetCore.Http;

namespace Core.Interfaces.Services;

public interface IImageService
{
    Task<List<Image>> AddUploadedImagesAsync(string entityType, long entityId, string imageType,
        List<IFormFile> uploadedImages);

    Task<Image> AddUploadedImageAsync(string entityType, long entityId, string imageType,
    IFormFile uploadedImage);

    //Task<List<Image>> AddImageUrlsAsync(string entityType, long entityId, string imageType, List<string> imageUrls);

    Task RemoveImages(List<long> imageIds);
    Task RemoveImage(long imageId);
}