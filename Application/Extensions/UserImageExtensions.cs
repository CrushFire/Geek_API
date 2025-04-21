using DataAccess.Entities;

namespace Application.Extensions;

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
                PasswordHash = user.PasswordHash,
                Description = user.Description,
                Posts = user.Posts,
                UserCommunities = user.UserCommunities,
                CreateAt = user.CreateAt,
                Images = user.Images
                    .Where(img => img.EntityTarget == "EntityUser" && img.EntityId == user.Id)
                    .ToList()
            });
    }
}