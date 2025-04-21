using DataAccess.Entities;

namespace Application.Extensions;

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
                PostCategories = post.PostCategories,
                Views = post.Views,
                Likes = post.Likes,
                Dislikes = post.Dislikes,
                CreateAt = post.CreateAt,
                Images = post.Images
                    .Where(img => img.EntityTarget == "EntityPost" && img.EntityId == post.Id)
                    .ToList()
            });
    }
}