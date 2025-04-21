using DataAccess.Entities;

namespace Application.Extensions;

public static class CommunityImageExtensions
{
    // Метод для добавления изображений для сообщества
    public static IQueryable<CommunityEntity> IncludeCommunityImages(this IQueryable<CommunityEntity> query)
    {
        return query
            .Select(community => new CommunityEntity
            {
                Id = community.Id,
                Name = community.Name,
                Description = community.Description,
                Categories = community.Categories,
                CreateAt = community.CreateAt,
                Images = community.Images
                    .Where(img => img.EntityTarget == "EntityCommunity" && img.EntityId == community.Id)
                    .ToList()
            });
    }
}