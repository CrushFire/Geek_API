using Core.Interfaces;
using Core.Models.Authorisation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Geek_API.Controllers.v1;

[ApiVersion("1.0")]
public class AuthController : CustomControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("login")]
    public async Task<IActionResult> LoginAsync([FromBody] LoginRequest loginRequest)
    {
        var result = await _authService.AuthenticateAsync(loginRequest);

        return result.IsSuccess
            ? Ok(ApiResponse.CreateSuccess(result.Data))
            : StatusCode(result.Error.StatusCode,
                ApiResponse.CreateFailure(result.Error.ErrorMessage));
    }

    [HttpPost("register")]
    public async Task<IActionResult> RegisterAsync([FromBody] RegisterRequest registerRequest)
    {
        var result = await _authService.RegisterAsync(registerRequest);

        return result.IsSuccess
            ? Ok(ApiResponse.CreateSuccess(new { Token = result.Data }))
            : StatusCode(result.Error.StatusCode, ApiResponse.CreateFailure(result.Error.ErrorMessage));
    }

    [Authorize]
    [HttpPatch("changePassword")]
    public async Task<IActionResult> ChangePasswordAsync([FromBody] PasswordUpdateRequest changePasswordRequest)
    {
        if (UserId == null)
            StatusCode(400, ApiResponse.CreateFailure("Ошибка токена"));

        var result = await _authService.ChangePasswordAsync(UserId.Value, changePasswordRequest.OldPassword,
            changePasswordRequest.NewPassword);

        return result.IsSuccess
            ? NoContent()
            : StatusCode(result.Error.StatusCode, ApiResponse.CreateFailure(result.Error.ErrorMessage));
    }
}