using Core.Models.Post;
using Core.Results;

namespace Core.Interfaces.Services;

public interface IPostService
{
    Task<ServiceResult<PostResponse>> GetByIdAsync(long id);
    Task<ServiceResult<bool>> HasBeenSeen(long id);
    Task<ServiceResult<List<PostResponse>>> GetByCommunityIdAsync(long communityId, int page, int pageSize);
    Task<ServiceResult<List<PostResponse>>> GetByUserIdAsync(long userId, int page, int pageSize);
    Task<ServiceResult<PostResponse>> AddAsync(PostAddRequest request, long userId);
    Task<ServiceResult<bool>> UpdateAsync(long id, PostUpdateRequest request, long userId);
    Task<ServiceResult<bool>> DeleteAsync(long id, long userId);
}