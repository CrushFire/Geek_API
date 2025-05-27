using Application.Utils;
using Core.Entities;
using Core.Interfaces;
using Core.Models;
using Core.Results;
using Microsoft.AspNetCore.Mvc;

namespace UI.Controllers;

[Route("User")]

public class UserController : CustomControllerBase
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet("/Popular/{UserId}")]
    public async Task<IActionResult> GetUserByIdAsync([FromRoute] long UserId)
    {
        if (UserId != base.UserId)
            return RedirectToAction("Login", "Auth");
        // Проверка авторизации
        //var auth = await _dataPage.GetByPageAsync("Authorization");

        //Штука для смены языка, как раз таки мой мидлвеар
        ViewBag.Language = HttpContext.Items["Language"] as string ?? "eng";
        //ViewBag.pageData = new SelectData(auth, ViewBag.Language);
        var result = await _userService.GetUserByIdAsync(UserId);

        return View("Popular", result.Data);
    }

    [HttpGet("/take-reactions/{userId}")]
    public async Task<IActionResult> GetUserReactions([FromRoute] long userId)
    {
        var result = await _userService.GetUserReactionsAsync(userId);

        if (!result.IsSuccess)
        {
            return NotFound("Таких реакий не найдено");
        }

        return Json(result.Data);
    }

    [HttpGet]
    public async Task<IActionResult> GetUsersAsync(int page = 1, int pageSize = 10)
    {
        var result = await _userService.GetUsersAsync(page, pageSize);
        return result.IsSuccess
            ? Ok(ApiResponse.CreateSuccess(result.Data))
            : StatusCode(result.Error.StatusCode, ApiResponse.CreateFailure(result.Error.ErrorMessage));
    }

    [HttpGet("byCommunity")]
    public async Task<IActionResult> GetUsersByCommunityAsync(long communityId, int page = 1, int pageSize = 10)
    {
        var result = await _userService.GetUsersByCommunityAsync(communityId, page, pageSize);
        return result.IsSuccess
            ? Ok(ApiResponse.CreateSuccess(result.Data))
            : StatusCode(result.Error.StatusCode, ApiResponse.CreateFailure(result.Error.ErrorMessage));
    }

    [HttpPut]
    public async Task<IActionResult> UpdateUserInfoAsync([FromBody] UserUpdateRequest user)
    {
        if (UserId == null)
            StatusCode(400, ApiResponse.CreateFailure("Ошибка токена"));

        var result = await _userService.UpdateUserInfoAsync(user, UserId.Value);

        return result.IsSuccess
            ? Ok(ApiResponse.CreateSuccess(result.Data))
            : StatusCode(result.Error.StatusCode, ApiResponse.CreateFailure(result.Error.ErrorMessage));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteUserAsync([FromRoute] long id)
    {
        var result = await _userService.DeleteUser(id);
        return result.IsSuccess
            ? NoContent()
            : StatusCode(result.Error.StatusCode, ApiResponse.CreateFailure(result.Error.ErrorMessage));
    }

    [HttpPut("banner")]
    public async Task<IActionResult> UpdateUserBannerAsync(List<IFormFile> images, long id)
    {
        if (UserId == null)
            StatusCode(400, ApiResponse.CreateFailure("Ошибка токена"));

        var result = await _userService.UploadBannerAsync(images, id);
        return result.IsSuccess
            ? NoContent()
            : StatusCode(result.Error.StatusCode, ApiResponse.CreateFailure(result.Error.ErrorMessage));
    }

    [HttpPut("{id}/avatar")]
    public async Task<IActionResult> UploadAvatarUserAsync(IFormFile image, [FromRoute] long id)
    {
        if (UserId == null)
            StatusCode(400, ApiResponse.CreateFailure("Ошибка токена"));

        var result = await _userService.UploadAvatarAsync(image, id);
        return result.IsSuccess
            ? Ok(ApiResponse.CreateSuccess(result.Data))
            : StatusCode(result.Error.StatusCode, ApiResponse.CreateFailure(result.Error.ErrorMessage));
    }
}