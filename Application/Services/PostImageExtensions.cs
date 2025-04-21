using DataAccess;
using DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Services
{
    public static class PostImageExtensions
    {
        // Метод для добавления изображений для поста
        public static IQueryable<EntityPost> IncludePostImages(this IQueryable<EntityPost> query)
        {
            return query
                .Select(post => new EntityPost
                {
                    Id = post.Id,
                    Title = post.Title,
                    Content = post.Content,
                    AuthorId = post.AuthorId,
                    Author = post.Author,
                    CommunityId = post.CommunityId,
                    Community = post.Community,
                    Categories = post.Categories,
                    Views = post.Views,
                    Likes = post.Likes,
                    Dislikes = post.Dislikes,
                    CreateAt = post.CreateAt,
                    Images = post.Images
                        .Where(img => img.EntityTarget == "EntityPost" && img.EntityId == post.Id)
                        .ToList()
                });
        }

        // Удаление изображений, связанных с постом
        public static async Task DeletePostImagesAsync(this IQueryable<EntityImage> query, int postId, DBContext context)
        {
            var imagesToDelete = await query
                .Where(img => img.EntityTarget == "EntityPost" && img.EntityId == postId)
                .ToListAsync();

            context.Images.RemoveRange(imagesToDelete);
            await context.SaveChangesAsync();
        }

        // Удаление всех изображений для постов
        public static async Task DeleteAllPostImagesAsync(this IQueryable<EntityImage> query, DBContext context)
        {
            var imagesToDelete = await query
                .Where(img => img.EntityTarget == "EntityPost")
                .ToListAsync();

            context.Images.RemoveRange(imagesToDelete);
            await context.SaveChangesAsync();
        }

        // Обновление изображения поста
        public static async Task UpdatePostImageAsync(this IQueryable<EntityImage> query, int postId, EntityImage newImage, DBContext context)
        {
            var imageToUpdate = await query
                .Where(img => img.EntityTarget == "EntityPost" && img.EntityId == postId)
                .FirstOrDefaultAsync();

            if (imageToUpdate != null)
            {
                imageToUpdate.ImageUrl = newImage.ImageUrl; // Обновляем URL или другие свойства
                imageToUpdate.Description = newImage.Description; // Если есть описание, обновляем его
                context.Images.Update(imageToUpdate);
                await context.SaveChangesAsync();
            }
        }

        // Добавление изображения для поста
        public static async Task AddPostImageAsync(this DBContext context, int postId, EntityImage image)
        {
            image.EntityTarget = "EntityPost";
            image.EntityId = postId;
            context.Images.Add(image);
            await context.SaveChangesAsync();
        }
    }
}
