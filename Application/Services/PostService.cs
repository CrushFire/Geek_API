using Application.Extensions;
using AutoMapper;
using Core.Entities;
using Core.Interfaces.Services;
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

        var postCategories = request.CategoryIds
            .Select(categoryId => new PostCategory
            {
                PostId = post.Id,
                CategoryId = categoryId
            })
            .ToList();

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

        var postCategories = request.CategoryIds
            .Select(categoryId => new PostCategory
            {
                PostId = post.Id,
                CategoryId = categoryId
            })
            .ToList();

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