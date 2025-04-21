using DataAccess;
using DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Services
{
    public static class UserImageExtensions
    {
        // Метод для добавления изображений для пользователя
        public static IQueryable<EntityUser> IncludeUserImages(this IQueryable<EntityUser> query)
        {
            return query
                .Select(user => new EntityUser
                {
                    Id = user.Id,
                    UserName = user.UserName,
                    Password = user.Password,
                    Description = user.Description,
                    AvatarUrl = user.AvatarUrl,
                    Posts = user.Posts,
                    NumberOfPosts = user.NumberOfPosts,
                    NumberOfComments = user.NumberOfComments,
                    CreateAt = user.CreateAt,
                    Banners = user.Banners
                        .Where(img => img.EntityTarget == "EntityUser" && img.EntityId == user.Id)
                        .ToList()
                });
        }

        // Удаление изображений, связанных с пользователем
        public static async Task DeleteUserImagesAsync(this IQueryable<EntityImage> query, int userId, DBContext context)
        {
            var imagesToDelete = await query
                .Where(img => img.EntityTarget == "EntityUser" && img.EntityId == userId)
                .ToListAsync();

            context.Images.RemoveRange(imagesToDelete);
            await context.SaveChangesAsync();
        }

        // Удаление всех изображений для пользователей
        public static async Task DeleteAllUserImagesAsync(this IQueryable<EntityImage> query, DBContext context)
        {
            var imagesToDelete = await query
                .Where(img => img.EntityTarget == "EntityUser")
                .ToListAsync();

            context.Images.RemoveRange(imagesToDelete);
            await context.SaveChangesAsync();
        }

        // Обновление изображения пользователя
        public static async Task UpdateUserImageAsync(this IQueryable<EntityImage> query, int userId, EntityImage newImage, DBContext context)
        {
            var imageToUpdate = await query
                .Where(img => img.EntityTarget == "EntityUser" && img.EntityId == userId)
                .FirstOrDefaultAsync();

            if (imageToUpdate != null)
            {
                imageToUpdate.ImageUrl = newImage.ImageUrl; // Обновляем URL или другие свойства
                imageToUpdate.Description = newImage.Description; // Если есть описание, обновляем его
                context.Images.Update(imageToUpdate);
                await context.SaveChangesAsync();
            }
        }

        // Добавление изображения для пользователя
        public static async Task AddUserImageAsync(this DBContext context, int userId, EntityImage image)
        {
            image.EntityTarget = "EntityUser";
            image.EntityId = userId;
            context.Images.Add(image);
            await context.SaveChangesAsync();
        }
    }
}
