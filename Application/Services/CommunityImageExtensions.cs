using DataAccess;
using DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Services
{
    public static class CommunityImageExtensions
    {
        // Метод для добавления изображений для сообщества
        public static IQueryable<EntityCommunity> IncludeCommunityImages(this IQueryable<EntityCommunity> query)
        {
            return query
                .Select(community => new EntityCommunity
                {
                    Id = community.Id,
                    Name = community.Name,
                    Description = community.Description,
                    NumberOfMembers = community.NumberOfMembers,
                    NumberOfPosts = community.NumberOfPosts,
                    AvatarUrl = community.AvatarUrl,
                    Owner = community.Owner,
                    Moderators = community.Moderators,
                    Categories = community.Categories,
                    CreateAt = community.CreateAt,
                    Banners = community.Banners
                        .Where(img => img.EntityTarget == "EntityCommunity" && img.EntityId == community.Id)
                        .ToList()
                });
        }

        // Удаление изображений, связанных с сообществом
        public static async Task DeleteCommunityImagesAsync(this IQueryable<EntityImage> query, int communityId, DBContext context)
        {
            var imagesToDelete = await query
                .Where(img => img.EntityTarget == "EntityCommunity" && img.EntityId == communityId)
                .ToListAsync();

            context.Images.RemoveRange(imagesToDelete);
            await context.SaveChangesAsync();
        }

        // Удаление всех изображений для сообществ
        public static async Task DeleteAllCommunityImagesAsync(this IQueryable<EntityImage> query, DBContext context)
        {
            var imagesToDelete = await query
                .Where(img => img.EntityTarget == "EntityCommunity")
                .ToListAsync();

            context.Images.RemoveRange(imagesToDelete);
            await context.SaveChangesAsync();
        }

        // Обновление изображения сообщества
        public static async Task UpdateCommunityImageAsync(this IQueryable<EntityImage> query, int communityId, EntityImage newImage, DBContext context)
        {
            var imageToUpdate = await query
                .Where(img => img.EntityTarget == "EntityCommunity" && img.EntityId == communityId)
                .FirstOrDefaultAsync();

            if (imageToUpdate != null)
            {
                imageToUpdate.ImageUrl = newImage.ImageUrl; // Обновляем URL или другие свойства
                imageToUpdate.Description = newImage.Description; // Если есть описание, обновляем его
                context.Images.Update(imageToUpdate);
                await context.SaveChangesAsync();
            }
        }

        // Добавление изображения для сообщества
        public static async Task AddCommunityImageAsync(this DBContext context, int communityId, EntityImage image)
        {
            image.EntityTarget = "EntityCommunity";
            image.EntityId = communityId;
            context.Images.Add(image);
            await context.SaveChangesAsync();
        }
    }
}
