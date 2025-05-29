using Application.Extensions;
using AutoMapper;
using Core.Entities;
using Core.Interfaces.Services;
using Core.Models;
using Core.Models.Filter;
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
        var post = await _context.Posts
            .Include(p => p.Author)
            .IncludePostImages()
            .Select(p => new PostWithLikes
            {
                Post = p,
                CountLikes = _context.Likes.Count(l => l.PostId == p.Id && l.IsLike),
                CountDislikes = _context.Likes.Count(l => l.PostId == p.Id && !l.IsLike)
            })
            .FirstOrDefaultAsync(p => p.Post.Id == id);

        if (post == null)
            return ServiceResult<PostResponse>.Failure("Пост не найден");

        var postResponse = _mapper.Map<PostResponse>(post);

        return ServiceResult<PostResponse>.Success(postResponse);
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


    public async Task<ServiceResult<List<PostResponse>>> GetByCommunityIdAsync(long communityId, int page, int pageSize)
    {
        var posts = await _context.Posts
            .Where(p => p.CommunityId == communityId)
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

    public async Task<ServiceResult<PostResponse>> AddAsync(PostAddRequest request, long userId)
    {
        var userExist = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
        if (userExist == null)
            ServiceResult<PostResponse>.Failure("Пользователь не найден");

        var communityExist = await _context.Posts.FirstOrDefaultAsync(c => c.Id == request.CommunityId);
        if (communityExist == null)
            ServiceResult<PostResponse>.Failure("Комьюнити не найдено");

        var post = _mapper.Map<Post>(request);
        post.AuthorId = userId;

        await _context.Posts.AddAsync(post);
        await _context.SaveChangesAsync();

        var postImages = await _imageService.AddUploadedImagesAsync(nameof(Post), post.Id, "banner", request.Images);
        post.Images = postImages;

        var postResponse = _mapper.Map<PostResponse>(post);
        return ServiceResult<PostResponse>.Success(postResponse);
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

        //TODO
        //Удалить старые создать новые
        //Отслеживать ли как-то существующие???

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

        if (post.AuthorId != userId)
            return ServiceResult<bool>.Failure("У вас нет прав на удаление");

        var imageIds = await _context.Images
            .Where(im => im.EntityId == post.Id && im.EntityTarget == nameof(Post))
            .Select(im => im.Id)
            .ToListAsync();

        await _imageService.RemoveImages(imageIds);

        _context.Posts.Remove(post);
        await _context.SaveChangesAsync();

        return ServiceResult<bool>.Success();
    }
}