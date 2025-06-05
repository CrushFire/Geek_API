using Core.Models;
using Core.Models.Community;
using Core.Models.Filter;
using Core.Models.Post;
using Core.Models.User;
using Core.Results;

namespace Core.Interfaces.Services
{
    public interface IFilterService
    {
        Task<ServiceResult<List<CommunityResponse>>> GetCommunitiesByFilter(ParametersFilter filter);
        Task<ServiceResult<List<PostResponse>>> GetPostsByFilter(ParametersFilter filter);
        Task<ServiceResult<List<UserSearchResponse>>> GetUsersByFilter(ParametersFilter filter);
    }
}