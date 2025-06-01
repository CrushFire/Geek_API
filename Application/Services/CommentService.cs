using AutoMapper;
using Core.Entities;
using Core.Interfaces.Services;
using Core.Models.Comment;
using Core.Results;
using DataAccess;
using Microsoft.EntityFrameworkCore;

namespace Application.Services;

public class CommentService : ICommentService
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;

    public CommentService(ApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<ServiceResult<CommentResponse>> GetByIdAsync(long id)
    {
        var comment = await _context.Comments
            .Include(c => c.Author)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (comment == null)
            return ServiceResult<CommentResponse>.Failure("Комментарий не найден");

        var commentResponse = _mapper.Map<CommentResponse>(comment);
        return ServiceResult<CommentResponse>.Success(commentResponse);
    }

    public async Task<ServiceResult<List<CommentResponse>>> GetByUserIdAsync(long userId, int page, int pageSize)
    {
        var comment = await _context.Comments
            .Where(c => c.AuthorId == userId)
            .Include(c => c.Author)
            .OrderByDescending(c => c.CreateAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var commentResponse = _mapper.Map<List<CommentResponse>>(comment);
        return ServiceResult<List<CommentResponse>>.Success(commentResponse);
    }

    public async Task<ServiceResult<CommentResponse>> AddAsync(CommentAddRequest request, long userId)
    {
        var postExist = await _context.Posts.AnyAsync(p => p.Id == request.PostId);
        if (!postExist)
            ServiceResult<CommentResponse>.Failure("Пост не найден");

        var userExist = await _context.Users.AnyAsync(p => p.Id == userId);
        if (!userExist)
            ServiceResult<CommentResponse>.Failure("Пользователь не найден");

        var comment = new Comment
        {
            AuthorId = userId,
            PostId = request.PostId,
            Content = request.Content
        };

        await _context.Comments.AddAsync(comment);
        await _context.SaveChangesAsync();

        var commentResponse = _mapper.Map<CommentResponse>(comment);
        return ServiceResult<CommentResponse>.Success(commentResponse);
    }

    public async Task<ServiceResult<bool>> UpdateAsync(long id, string content, long userId)
    {
        var comment = await _context.Comments
            .FirstOrDefaultAsync(c => c.Id == id);

        if (comment == null)
            return ServiceResult<bool>.Failure("Комментарий не найден");

        if (comment.AuthorId != userId)
            return ServiceResult<bool>.Failure("У вас нет прав на изменение");

        comment.Content = content;
        _context.Comments.Update(comment);
        await _context.SaveChangesAsync();

        return ServiceResult<bool>.Success();
    }

    public async Task<ServiceResult<bool>> DeleteAsync(long id, long userId)
    {
        var comment = await _context.Comments
            .FirstOrDefaultAsync(c => c.Id == id);

        if (comment == null)
            return ServiceResult<bool>.Failure("Комментарий не найден");

        if (comment.AuthorId != userId)
            return ServiceResult<bool>.Failure("У вас нет прав на изменение");

        _context.Comments.Remove(comment);
        await _context.SaveChangesAsync();

        return ServiceResult<bool>.Success();
    }

    public async Task<ServiceResult<List<CommentResponse>>> GetByPostIdAsync(long postId, int page, int pageSize)
    {
        var comment = await _context.Comments
            .Where(c => c.PostId == postId)
            .Include(c => c.Author)
                .ThenInclude(a => a.Images)
            .OrderByDescending(c => c.CreateAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();


        var commentResponse = _mapper.Map<List<CommentResponse>>(comment);
        return ServiceResult<List<CommentResponse>>.Success(commentResponse);
    }
}