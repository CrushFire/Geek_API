using Core.Models;
using Core.Models.Community;
using Core.Results;

namespace Core.Interfaces.Services;

public interface ICommunityService
{
    Task<ServiceResult<CommunityResponse>> AddCommunityAsync(CommunityAddRequest communityAddRequest, long authorId);
    Task<ServiceResult<bool>> DeleteCommunityAsync(long id);
    Task<ServiceResult<CommunityResponse>> GetByIdAsync(long id);
    Task<ServiceResult<List<CommunityResponse>>> GetCommunitiesSubscribeUser(PaginationRequest paginationRequest, long userId);
    Task<ServiceResult<List<CommunityResponse>>> GetCommunitiesCreatedUser(PaginationRequest paginationRequest, long userId);
    Task<ServiceResult<List<CommunityResponse>>> GetCommunityAsync(int page = 1, int pageSize = 10);
    Task<ServiceResult<string>> SubOrNo(long userId, long communityId);
    Task<ServiceResult<bool>> SubsribeAsync(long userId, long communityId);
    Task<ServiceResult<bool>> UnSubscribeAsync(long userId, long communityId);
    Task<ServiceResult<bool>> UpdateCommunityAsync(CommunityAddRequest communityAddRequest, long id);
}