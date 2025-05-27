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
                CommunityCategories = community.CommunityCategories,
                Description = community.Description,
                CreateAt = community.CreateAt,
                Images = community.Images
                    .Where(img => img.EntityTarget == nameof(Community) && img.EntityId == community.Id)
                    .ToList()
            });
    }
}