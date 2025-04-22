using Core.Entities;
using Core.Models;
using Core.Results;
using Microsoft.AspNetCore.Http;

namespace Core.Interfaces;

public interface IUserService
{
    Task<ServiceResult<bool>> DeleteUser(long id);
    Task<ServiceResult<UserResponse>> GetUserByIdAsync(long id);
    Task<ServiceResult<List<UserResponse>>> GetUsersByCommunityAsync(long communityId, int page = 1, int pageSize = 10);
    Task<ServiceResult<List<UserResponse>>> GetUsersAsync(int page = 1, int limit = 10);
    Task<ServiceResult<bool>> UpdateUserInfoAsync(UserUpdateRequest user, long id);
    Task<ServiceResult<Image>> UploadAvatarAsync(string avatarPath, long userId);
    Task<ServiceResult<Image>> UploadBannerAsync(List<IFormFile> images, long userId);
}