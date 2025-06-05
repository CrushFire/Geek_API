using Core.Entities;
using Core.Interfaces.Services;
using DataAccess;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Application.Services;

public class ImageService : IImageService
{
    private readonly ApplicationDbContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public ImageService(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
    {
        _context = context;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<List<Image>> AddUploadedImagesAsync(
        string entityType,
        long entityId,
        string imageType,
        List<IFormFile> uploadedImages)
    {
        if (uploadedImages == null || !uploadedImages.Any())
            throw new ArgumentException("Список загруженных изображений пуст.");

        var request = _httpContextAccessor.HttpContext?.Request; //?
        if (request == null)
            throw new Exception("Не удалось получить адрес сервера");

        var baseUrl = $"{request.Scheme}://{request.Host}";
        var resultImages = new List<Image>();

        foreach (var file in uploadedImages)
        {
            if (file.Length == 0)
                continue;

            var extension = Path.GetExtension(file.FileName);
            var fileName = $"{Guid.NewGuid()}{extension}";
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", fileName);

            await using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            resultImages.Add(new Image
            {
                ImageUrl = $"{baseUrl}/images/{fileName}",
                EntityId = entityId,
                EntityTarget = entityType,
                ImageType = imageType
            });
        }

        await _context.Images.AddRangeAsync(resultImages);
        await _context.SaveChangesAsync();

        return resultImages;
    }

    public async Task<Image> AddUploadedImageAsync(
    string entityType,
    long entityId,
    string imageType,
    IFormFile uploadedImage)
    {
        if (uploadedImage == null)
            throw new ArgumentException("Список загруженных изображений пуст.");

        var request = _httpContextAccessor.HttpContext?.Request; //?
        if (request == null)
            throw new Exception("Не удалось получить адрес сервера");

        var baseUrl = $"{request.Scheme}://{request.Host}";

            var extension = Path.GetExtension(uploadedImage.FileName);
            var fileName = $"{Guid.NewGuid()}{extension}";
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", fileName);

            await using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await uploadedImage.CopyToAsync(stream);
            }

            var resultImage = new Image
            {
                ImageUrl = $"{baseUrl}/images/{fileName}",
                EntityId = entityId,
                EntityTarget = entityType,
                ImageType = imageType
            };

        await _context.Images.AddAsync(resultImage);
        await _context.SaveChangesAsync();

        return resultImage;
    }

    //public async Task<List<Image>> AddImageUrlsAsync(
    //    string entityType,
    //    long entityId,
    //    string imageType,
    //    List<string> imageUrls)
    //{
    //    if (imageUrls == null || !imageUrls.Any())
    //        throw new ArgumentException("Список URL изображений пуст.");

    //    var resultImages = imageUrls
    //        .Where(url => !string.IsNullOrWhiteSpace(url))
    //        .Select(url => new Image
    //        {
    //            ImageUrl = url,
    //            EntityId = entityId,
    //            EntityTarget = entityType,
    //            ImageType = imageType
    //        })
    //        .ToList();

    //    await _context.Images.AddRangeAsync(resultImages);
    //    await _context.SaveChangesAsync();

    //    return resultImages;
    //}


    public async Task<bool> RemoveImages(List<long> imageIds)
    {
        var images = await _context.Images
            .Where(im => imageIds.Contains(im.Id))
            .ToListAsync();

        foreach (var image in images)
        {
            var imageUrl = image.ImageUrl;
            var startIndex = imageUrl.IndexOf("images/") + "images/".Length;
            var fileName = imageUrl.Substring(startIndex);

            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", fileName);
            if (File.Exists(filePath)) File.Delete(filePath);
        }

        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<bool> RemoveImage(long imageId)
    {
        var image = await _context.Images
            .Where(im => im.Id == imageId)
            .FirstOrDefaultAsync();

            var imageUrl = image.ImageUrl;
            var startIndex = imageUrl.IndexOf("images/") + "images/".Length;
            var fileName = imageUrl.Substring(startIndex);

            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", fileName);
            if (File.Exists(filePath)) File.Delete(filePath);

        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<bool> RemoveImageFromServer(long imageId)
    {
        var image = await _context.Images
            .Where(im => im.Id == imageId)
            .FirstOrDefaultAsync();

        var imageUrl = image.ImageUrl;
        var startIndex = imageUrl.IndexOf("images/") + "images/".Length;
        var fileName = imageUrl.Substring(startIndex);

        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", fileName);
        if (File.Exists(filePath)) File.Delete(filePath);

        _context.Images.Remove(image);
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<bool> RemoveImagesFromServer(List<long> imageIds)
    {
        var images = await _context.Images
            .Where(im => imageIds.Contains(im.Id))
            .ToListAsync();

        foreach (var image in images)
        {
            var imageUrl = image.ImageUrl;
            var startIndex = imageUrl.IndexOf("images/") + "images/".Length;
            var fileName = imageUrl.Substring(startIndex);

            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", fileName);
            if (File.Exists(filePath)) File.Delete(filePath);
        }

        _context.Images.RemoveRange(images);
        await _context.SaveChangesAsync();

        return true;
    }
}