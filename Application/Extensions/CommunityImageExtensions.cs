using DataAccess.Entities;

namespace Application.Extensions;

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
                Users = community.Users,
                Categories = community.Categories,
                CreateAt = community.CreateAt,
                Images = community.Images
                    .Where(img => img.EntityTarget == "EntityCommunity" && img.EntityId == community.Id)
                    .ToList()
            });
    }
}