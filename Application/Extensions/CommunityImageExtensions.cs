using Core.Entities;

namespace Application.Extensions;

public static class CommunityImageExtensions
{
    // Метод для добавления изображений для сообщества
    public static IQueryable<Community> IncludeCommunityImages(this IQueryable<Community> query)
    {
        return query
            .Select(community => new Community
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