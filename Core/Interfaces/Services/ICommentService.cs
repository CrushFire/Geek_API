using Core.Models.Comment;
using Core.Results;

namespace Core.Interfaces.Services;

public interface ICommentService
{
    Task<ServiceResult<CommentResponse>> GetByIdAsync(long id);
    Task<ServiceResult<List<CommentResponse>>> GetByUserIdAsync(long userId, int page, int pageSize);
    Task<ServiceResult<List<CommentResponse>>> GetByPostIdAsync(long postId, int page, int pageSize);
    Task<ServiceResult<CommentResponse>> AddAsync(CommentAddRequest request, long userId);
    Task<ServiceResult<bool>> UpdateAsync(long id, string content, long userId);
    Task<ServiceResult<bool>> DeleteAsync(long id, long userId);
}