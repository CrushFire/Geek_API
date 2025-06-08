using Application.Extensions;
using AutoMapper;
using Core.Entities;
using Core.Interfaces.Services;
using Core.Models;
using Core.Models.Comment;
using Core.Models.Community;
using Core.Models.Filter;
using Core.Models.Image;
using Core.Models.Post;
using Core.Results;
using DataAccess;
using Microsoft.EntityFrameworkCore;

namespace Application.Services;

public class PostService : IPostService
{
    private readonly ApplicationDbContext _context;
    private readonly IImageService _imageService;
    private readonly IMapper _mapper;

    public PostService(ApplicationDbContext context, IMapper mapper, IImageService imageService)
    {
        _context = context;
        _mapper = mapper;
        _imageService = imageService;
    }

    public async Task<ServiceResult<PostResponse>> GetByIdAsync(long id)
    {
        var p = await _context.Posts
            .Include(p => p.Author)
            .Include(p => p.PostCategories)
                .ThenInclude(pc => pc.Category)
            .Include(p => p.Community)
            .IncludePostImages()
            .Select(p => new
            {
                CommunityName = p.Community != null ? p.Community.Name : null,
                CommunityAvatar = _context.Images.Where(i => i.EntityTarget == "Community" && i.EntityId == p.CommunityId && i.ImageType == "avatar").FirstOrDefault(),
                UserAvatar = _context.Images.Where(i => i.EntityTarget == "User" && i.EntityId == p.AuthorId && i.ImageType == "avatar").FirstOrDefault(),
                Post = p,
            })
            .FirstOrDefaultAsync(p => p.Post.Id == id);

        var post = new PostWithLikes()
        {
            Post = p.Post,
            CountLikes = _context.Likes.Count(l => l.IsLike && l.PostId == id),
            CountDislikes = _context.Likes.Count(l => !l.IsLike && l.PostId == id),
            CategoriesRu = p.Post.PostCategories
                .Select(pc => pc.Category.Title)
                .ToList(),
            CategoriesEng = p.Post.PostCategories
                .Select(pc => pc.Category.EngTitle)
                .ToList(),
            CommunityAvatar = p.CommunityAvatar?.ImageUrl,
            CommunityName = p.CommunityName,
            UserAvatar = p.UserAvatar?.ImageUrl,
            CountComments = _context.Comments.Where(c => c.PostId == id).Count(),
            PostImages = _context.Images.Where(i => i.EntityTarget == "Post" && i.EntityId == p.Post.Id && i.ImageType == "image").Select(p => p.ImageUrl).ToList()
        };

        var postResponse = _mapper.Map<PostResponse>(post);

        return ServiceResult<PostResponse>.Success(postResponse);
    }

    public async Task<ServiceResult<PostWithCategoriesResponse>> GetByIdWithCategoriesAsync(long id)
    {
        var p = await _context.Posts
            .Include(p => p.PostCategories)
                .ThenInclude(pc => pc.Category)
            .Include(p => p.Community)
            .Include(p => p.Author)
            .FirstOrDefaultAsync(p => p.Id == id);

        var postImages = await _context.Images
            .Where(i => i.EntityTarget == "Post" && i.EntityId == id && i.ImageType == "image")
            .Select(i => new ImageIdResponse
            {
                Id = i.Id,
                Url = i.ImageUrl
            })
            .ToListAsync();

        var response = new PostWithCategoriesResponse
        {
            Title = p.Title,
            Content = p.Content,
            CommunityId = p.Community?.Id,
            CategoriesIds = p.PostCategories.Select(pc => pc.CategoryId).ToList(),
            PostImages = postImages
        };

        return ServiceResult<PostWithCategoriesResponse>.Success(response);
    }

    public async Task<ServiceResult<bool>> HasBeenSeen(long id)
    {
        var post = await _context.Posts.FirstOrDefaultAsync(p => p.Id == id);

        if(post == null)
        {
            return ServiceResult<bool>.Failure("Пост не найден");
        }

        post.Views++;

        _context.SaveChanges();

        return ServiceResult<bool>.Success(true);
    }

    public async Task<ServiceResult<PostReactionResult>> PostReactionsAsync(PostReaction reaction)
    {
        var existing = await _context.Likes
            .FirstOrDefaultAsync(r => r.PostId == reaction.PostId && r.UserId == reaction.UserId);

        if (existing != null)
        {
            if (existing.IsLike == reaction.IsLike)
            {
                _context.Likes.Remove(existing);
                await _context.SaveChangesAsync();
                return await BuildReactionResult(reaction.PostId, null);
            }
            else
            {
                existing.IsLike = reaction.IsLike;
                await _context.SaveChangesAsync();
                return await BuildReactionResult(reaction.PostId, reaction.IsLike);
            }
        }
        else
        {
            await _context.Likes.AddAsync(new Like
            {
                PostId = reaction.PostId,
                UserId = reaction.UserId,
                IsLike = reaction.IsLike
            });
        }

        await _context.SaveChangesAsync();
        return await BuildReactionResult(reaction.PostId, reaction.IsLike);
    }

    private async Task<ServiceResult<PostReactionResult>> BuildReactionResult(int postId, bool? userReacted)
    {
        var likes = await _context.Likes.CountAsync(r => r.PostId == postId && r.IsLike);
        var dislikes = await _context.Likes.CountAsync(r => r.PostId == postId && !r.IsLike);

        var resultReactions = new PostReactionResult
        {
            Likes = likes,
            Dislikes = dislikes,
            UserReaction = userReacted
        };

        return ServiceResult<PostReactionResult>.Success(resultReactions);
    }

    public async Task<ServiceResult<List<PostResponse>>> GetUserLikesPost(PaginationRequest paginationRequest, long id)
    {
        var likedPosts = await _context.Posts
            .Include(p => p.Author)
            .Include(p => p.PostCategories)
                .ThenInclude(pc => pc.Category)
            .Include(p => p.Community)
            .Include(p => p.Reactions).Where(p => p.Reactions.Any(r => r.UserId == id && r.IsLike == true))
            .IncludePostImages().OrderByDescending(p => p.CreateAt)
            .Select(p => new
            {
                CommunityName = p.Community.Name,
                CommunityAvatar = _context.Images.Where(i => i.EntityTarget == "Community" && i.EntityId == p.CommunityId && i.ImageType == "avatar").FirstOrDefault(),
                UserAvatar = _context.Images.Where(i => i.EntityTarget == "User" && i.EntityId == p.AuthorId && i.ImageType == "avatar").FirstOrDefault(),
                Post = p,
                LikeCount = _context.Likes.Count(l => l.PostId == p.Id && l.IsLike),
                DislikeCount = _context.Likes.Count(l => l.PostId == p.Id && !l.IsLike),
                CommentsCount = _context.Comments.Count(c => c.PostId == p.Id),
            }).ToListAsync();

        int page = paginationRequest.Page > 0 ? paginationRequest.Page : 1;
        int pageSize = paginationRequest.PageSize > 0 ? paginationRequest.PageSize : 10;
        int skip = (page - 1) * pageSize;

        var likedPostsPag = likedPosts.Skip(skip).Take(pageSize);

        var posts = likedPostsPag.Select(p => new PostWithLikes()
        {
            Post = p.Post,
            CountLikes = p.LikeCount,
            CountDislikes = p.DislikeCount,
            CategoriesRu = p.Post.PostCategories
                .Select(pc => pc.Category.Title)
                .ToList(),
            CategoriesEng = p.Post.PostCategories
                .Select(pc => pc.Category.EngTitle)
                .ToList(),
            CommunityAvatar = p.CommunityAvatar?.ImageUrl,
            UserAvatar = p.UserAvatar?.ImageUrl,
            CountComments = p.CommentsCount,
            PostImages = _context.Images.Where(i => i.EntityTarget == "Post" && i.EntityId == p.Post.Id && i.ImageType == "image").Select(p => p.ImageUrl).ToList()
        });

        var mappingPost = _mapper.Map<List<PostResponse>>(posts);

        return ServiceResult<List<PostResponse>>.Success(mappingPost);
    }

    public async Task<ServiceResult<List<PostResponse>>> GetPostPublishUser(PaginationRequest paginationRequest, long id)
    {
        var likedPosts = await _context.Posts
            .Include(p => p.Author).Where(p => p.AuthorId == id)
            .Include(p => p.PostCategories)
                .ThenInclude(pc => pc.Category)
            .Include(p => p.Community)
            .Include(p => p.Reactions)
            .IncludePostImages().OrderByDescending(p => p.CreateAt)
            .Select(p => new
            {
                CommunityName = p.Community.Name,
                CommunityAvatar = _context.Images.Where(i => i.EntityTarget == "Community" && i.EntityId == p.CommunityId && i.ImageType == "avatar").FirstOrDefault(),
                UserAvatar = _context.Images.Where(i => i.EntityTarget == "User" && i.EntityId == p.AuthorId && i.ImageType == "avatar").FirstOrDefault(),
                Post = p,
                LikeCount = _context.Likes.Count(l => l.PostId == p.Id && l.IsLike),
                DislikeCount = _context.Likes.Count(l => l.PostId == p.Id && !l.IsLike),
                CommentsCount = _context.Comments.Count(c => c.PostId == p.Id),
            }).ToListAsync();

        int page = paginationRequest.Page > 0 ? paginationRequest.Page : 1;
        int pageSize = paginationRequest.PageSize > 0 ? paginationRequest.PageSize : 10;
        int skip = (page - 1) * pageSize;

        var likedPostsPag = likedPosts.Skip(skip).Take(pageSize);

        var posts = likedPostsPag.Select(p => new PostWithLikes()
        {
            Post = p.Post,
            CountLikes = p.LikeCount,
            CountDislikes = p.DislikeCount,
            CategoriesRu = p.Post.PostCategories
                .Select(pc => pc.Category.Title)
                .ToList(),
            CategoriesEng = p.Post.PostCategories
                .Select(pc => pc.Category.EngTitle)
                .ToList(),
            CommunityAvatar = p.CommunityAvatar?.ImageUrl,
            UserAvatar = p.UserAvatar?.ImageUrl,
            CountComments = p.CommentsCount,
            PostImages = _context.Images.Where(i => i.EntityTarget == "Post" && i.EntityId == p.Post.Id && i.ImageType == "image").Select(p => p.ImageUrl).ToList()
        });

        var mappingPost = _mapper.Map<List<PostResponse>>(posts);

        return ServiceResult<List<PostResponse>>.Success(mappingPost);
    }


    public async Task<ServiceResult<List<PostResponse>>> GetByCommunityIdAsync(long communityId, PaginationRequest paginationRequest)
    {
        var postsByCommunity = await _context.Posts
            .Include(p => p.Author)
            .Include(p => p.PostCategories)
                .ThenInclude(pc => pc.Category)
            .Include(p => p.Community).Where(p => p.CommunityId == communityId)
            .Include(p => p.Reactions)
            .IncludePostImages().OrderByDescending(p => p.CreateAt)
            .Select(p => new
            {
                CommunityName = p.Community.Name,
                CommunityAvatar = _context.Images.Where(i => i.EntityTarget == "Community" && i.EntityId == p.CommunityId && i.ImageType == "avatar").FirstOrDefault(),
                UserAvatar = _context.Images.Where(i => i.EntityTarget == "User" && i.EntityId == p.AuthorId && i.ImageType == "avatar").FirstOrDefault(),
                Post = p,
                LikeCount = _context.Likes.Count(l => l.PostId == p.Id && l.IsLike),
                DislikeCount = _context.Likes.Count(l => l.PostId == p.Id && !l.IsLike),
                CommentsCount = _context.Comments.Count(c => c.PostId == p.Id),
            }).ToListAsync();

        int page = paginationRequest.Page > 0 ? paginationRequest.Page : 1;
        int pageSize = paginationRequest.PageSize > 0 ? paginationRequest.PageSize : 10;
        int skip = (page - 1) * pageSize;

        var postsByCommunityPag = postsByCommunity.Skip(skip).Take(pageSize);

        var posts = postsByCommunityPag.Select(p => new PostWithLikes()
        {
            Post = p.Post,
            CountLikes = p.LikeCount,
            CountDislikes = p.DislikeCount,
            CategoriesRu = p.Post.PostCategories
                .Select(pc => pc.Category.Title)
                .ToList(),
            CategoriesEng = p.Post.PostCategories
                .Select(pc => pc.Category.EngTitle)
                .ToList(),
            CommunityAvatar = p.CommunityAvatar?.ImageUrl,
            UserAvatar = p.UserAvatar?.ImageUrl,
            CountComments = p.CommentsCount,
            PostImages = _context.Images.Where(i => i.EntityTarget == "Post" && i.EntityId == p.Post.Id && i.ImageType == "image").Select(p => p.ImageUrl).ToList()
        });

        var mappingPost = _mapper.Map<List<PostResponse>>(posts);

        return ServiceResult<List<PostResponse>>.Success(mappingPost);
    }

    public async Task<ServiceResult<List<PostResponse>>> GetByUserIdAsync(long userId, int page, int pageSize)
    {
        var posts = await _context.Posts
            .Where(p => p.AuthorId == userId)
            .Include(p => p.Author)
            .IncludePostImages()
            .OrderByDescending(p => p.CreateAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(p => new PostWithLikes
            {
                Post = p,
                CountLikes = _context.Likes.Count(l => l.PostId == p.Id && l.IsLike),
                CountDislikes = _context.Likes.Count(l => l.PostId == p.Id && !l.IsLike)
            })
            .ToListAsync();

        var postResponses = _mapper.Map<List<PostResponse>>(posts);
        return ServiceResult<List<PostResponse>>.Success(postResponses);
    }

    public async Task<ServiceResult<long>> AddAsync(PostAddRequest request, long userId)
    {
        var userExist = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
        if (userExist == null)
            return ServiceResult<long>.Failure("Пользователь не найден");

        var post = _mapper.Map<Post>(request);
        post.AuthorId = userId;
        post.CommunityId = request.CommunityId;

        await _context.Posts.AddAsync(post);
        await _context.SaveChangesAsync();

        var postCategories = request.CategoriesIds.Select(catId => new PostCategory
        {
            PostId = post.Id,
            CategoryId = catId
        });

        if (request.Images != null && request.Images.Any())
        {
            var postImages = await _imageService.AddUploadedImagesAsync("Post", post.Id, "image", request.Images);
            post.Images = postImages;
        }

        await _context.PostCategories.AddRangeAsync(postCategories);
        await _context.SaveChangesAsync();

        return ServiceResult<long>.Success(post.Id);
    }

    public async Task<ServiceResult<bool>> UpdateAsync(long id, PostUpdateRequest request, long userId)
    {
        var post = await _context.Posts
            .FirstOrDefaultAsync(p => p.Id == id);

        if (post == null)
            return ServiceResult<bool>.Failure("Пост не найден");

        if (post.AuthorId != userId)
            return ServiceResult<bool>.Failure("У вас нет прав на изменение");

        post.Title = request.Title;
        post.Content = request.Content;
        if(request.ImagesToRemove != null && request.ImagesToRemove.Any())
        {
            await _imageService.RemoveImagesFromServer(request.ImagesToRemove);
        }
        if (request.NewImages != null && request.NewImages.Any())
        {
            await _imageService.AddUploadedImagesAsync("Post", post.Id, "image", request.NewImages);
        }

        var postCategories = await _context.PostCategories.Where(pc => pc.PostId == post.Id).ToListAsync();
        _context.PostCategories.RemoveRange(postCategories);

        var newPostCategories = request.Categories
                .Select(catId => new PostCategory
                {
                    PostId = post.Id,
                    CategoryId = catId
                })
                .ToList();

        _context.PostCategories.AddRange(newPostCategories);

        _context.Posts.Update(post);
        await _context.SaveChangesAsync();

        return ServiceResult<bool>.Success();
    }

    public async Task<ServiceResult<bool>> DeleteAsync(long id, long userId)
    {
        var post = await _context.Posts
            .FirstOrDefaultAsync(p => p.Id == id);

        if (post == null)
            return ServiceResult<bool>.Failure("Пост не найден");

        var isAdmin = await _context.Users.AnyAsync(u => u.Id == userId);
        if (post.AuthorId != userId && !isAdmin)
            return ServiceResult<bool>.Failure("У вас нет прав на удаление");

        var imageIds = await _context.Images
            .Where(im => im.EntityId == post.Id && im.EntityTarget == nameof(Post))
            .Select(im => im.Id)
            .ToListAsync();

        await _imageService.RemoveImages(imageIds);

        _context.Posts.Remove(post);
        await _context.SaveChangesAsync();

        return ServiceResult<bool>.Success(true);
    }
}