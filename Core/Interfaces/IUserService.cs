using Core.Models;
using Core.Results;
using Microsoft.AspNetCore.Http;

namespace Core.Interfaces;

public interface IUserService
{
    Task<ServiceResult<int>> AddUserAsync(UserRequest user);
    Task<ServiceResult<bool>> DeleteUser(int id);
    Task<ServiceResult<UserResponse>> GetUserAsync(int id);
    Task<ServiceResult<List<UserResponse>>> GetUsersAsync(int page = 1, int limit = 10);
    Task<ServiceResult<bool>> UpdateUserAsync(UserRequest user, int id);
    Task<ServiceResult<string>> UploadAvatar(IFormFile avatar, int userId);
}