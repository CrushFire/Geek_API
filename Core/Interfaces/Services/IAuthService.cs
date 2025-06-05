using Core.Models.Authorisation;
using Core.Results;

namespace Core.Interfaces;

public interface IAuthService
{
    Task<ServiceResult<string>> RegisterAsync(RegisterRequest userRequest);

    Task<ServiceResult<(string token, long id)>> AuthenticateAsync(LoginRequest userData);
    Task<ServiceResult<bool>> ChangePasswordAsync(long userId, string oldPassword, string newPassword);
}