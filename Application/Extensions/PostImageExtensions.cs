using Core.Entities;

namespace Application.Extensions;

public static class PostImageExtensions
{
    // Метод для добавления изображений для поста
    public static IQueryable<Post> IncludePostImages(this IQueryable<Post> query)
    {
        return query
            .Select(post => new Post
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
                CreateAt = post.CreateAt,
                Images = post.Images
                    .Where(img => img.EntityTarget == nameof(Post) && img.EntityId == post.Id)
                    .ToList()
            });
    }
}