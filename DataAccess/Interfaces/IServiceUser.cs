using Core.Models;
using Microsoft.AspNetCore.Http;

namespace DataAccess.Interfaces
{
    public interface IServiceUser
    {
        Task<int> AddUserAsync(DtoRequestUser _user);
        Task<bool> DeleteUser(int id);
        Task<DtoResponseUser> GetUserAsync(int id);
        Task<List<DtoResponseUser>> GetUsersAsync(int page = 1, int pageSize = 10);
        bool UpdateUser(DtoRequestIdUser _user, int id);
        Task<string> UploadAvatar(IFormFile avatar, int userId);
    }
}