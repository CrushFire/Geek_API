using Core.Models;
using Core.Models.Filter;
using Core.Models.Post;
using Core.Results;

namespace Core.Interfaces.Services;

public interface IPostService
{
    Task<ServiceResult<PostResponse>> GetByIdAsync(long id);
    Task<ServiceResult<bool>> HasBeenSeen(long id);
    Task<ServiceResult<PostReactionResult>> PostReactionsAsync(PostReaction reaction);
    Task<ServiceResult<List<PostResponse>>> GetUserLikesPost(PaginationRequest paginationRequest, long id);
    Task<ServiceResult<List<PostResponse>>> GetPostPublishUser(PaginationRequest paginationRequest, long id);
    Task<ServiceResult<List<PostResponse>>> GetByCommunityIdAsync(long communityId, PaginationRequest paginationRequest);
    Task<ServiceResult<List<PostResponse>>> GetByUserIdAsync(long userId, int page, int pageSize);
    Task<ServiceResult<PostAddRequest>> AddAsync(PostAddRequest request, long userId);
    Task<ServiceResult<bool>> UpdateAsync(long id, PostUpdateRequest request, long userId);
    Task<ServiceResult<bool>> DeleteAsync(long id, long userId);
}