using Application.Services;
using Application.Utils;
using Core.Entities;
using Core.Enums;
using Core.Interfaces;
using Core.Interfaces.Services;
using Core.Models;
using Core.Models.Filter;
using Core.Results;
using Microsoft.AspNetCore.Mvc;

namespace UI.Controllers;

[Route("User")]

public class UserController : CustomControllerBase
{
    private readonly IUserService _userService;
    private readonly IErrorMessages _errorMessages;
    private readonly IFilterService _filterService;
    private readonly IDataPageService _dataService;

    public UserController(IUserService userService, IErrorMessages errorMessages, IFilterService filterService, IDataPageService dataService)
    {
        _userService = userService;
        _errorMessages = errorMessages;
        _filterService = filterService;
        _dataService = dataService;
    }

    [HttpGet("/Popular/{UserId}")]
    public async Task<IActionResult> GetUserByIdAsync([FromRoute] long UserId)
    {
        if (UserId != base.UserId)
            return RedirectToAction("Login", "Auth");

        var pop = await _dataService.GetByPageAsync("PostCard");

        ViewBag.Language = HttpContext.Items["Language"] as string ?? "eng";
        ViewBag.pageData = new SelectData(pop, ViewBag.Language);

        var result = await _userService.GetUserByIdAsync(UserId);

        return View("Popular", result.Data);
    }

    [HttpGet("/New/{UserId}")]
    public async Task<IActionResult> GetUserByIdForNewAsync([FromRoute] long UserId)
    {
        if (UserId != base.UserId)
            return RedirectToAction("Login", "Auth");

        var pop = await _dataService.GetByPageAsync("PostCard");

        ViewBag.Language = HttpContext.Items["Language"] as string ?? "eng";
        ViewBag.pageData = new SelectData(pop, ViewBag.Language);
        //ViewBag.pageData = new SelectData(auth, ViewBag.Language);
        var result = await _userService.GetUserByIdAsync(UserId);

        return View("New", result.Data);
    }

    [HttpGet("/Home/{UserId}")]
    public async Task<IActionResult> GetUserByIdForHomeAsync([FromRoute] long UserId)
    {
        if (UserId != base.UserId)
            return RedirectToAction("Login", "Auth");

        var home = await _dataService.GetByPageAsync("Home");
        // Проверка авторизации
        //var auth = await _dataPage.GetByPageAsync("Authorization");

        //Штука для смены языка, как раз таки мой мидлвеар
        ViewBag.Language = HttpContext.Items["Language"] as string ?? "eng";
        ViewBag.pageData = new SelectData(home, ViewBag.Language);
        //ViewBag.pageData = new SelectData(auth, ViewBag.Language);
        var result = await _userService.GetUserByIdAsync(UserId);

        return View("Home", result.Data);
    }

    [HttpGet("/UserPage/{userId}")]
    public async Task<IActionResult> GetUserByForUserPageAsync([FromRoute] long userId)
    {
        if(userId == UserId.Value)
        {
            return Redirect($"/Home/{userId}");
        }
        Console.WriteLine(UserId + "+" + userId);

        var userPage = await _dataService.GetByPageAsync("UserPage");
        //Штука для смены языка, как раз таки мой мидлвеар
        ViewBag.Language = HttpContext.Items["Language"] as string ?? "eng";
        ViewBag.pageData = new SelectData(userPage, ViewBag.Language);
        ViewBag.userIdToken = UserId.Value;
        //ViewBag.pageData = new SelectData(auth, ViewBag.Language);
        var result = await _userService.GetUserByIdAsync(userId);

        return View("UserPage", result.Data);
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

    [HttpPost("/UpdateProfile")]
    public async Task<IActionResult> UpdateUserInfoAsync([FromForm] UserUpdateRequest userRequest)
    {
        if (UserId != base.UserId)
            return RedirectToAction("Login", "Auth");

        var result = await _userService.UpdateUserInfoAsync(userRequest, UserId.Value);

        if (!result.IsSuccess)
        {
            return BadRequest("Не удалось обновить данные юзера!");
        }

        return Json(result.Data);
    }

    [HttpGet("Search")]

    public async Task<IActionResult> Search()
    {
        var search = await _dataService.GetByPageAsync("Search");

        ViewBag.Language = HttpContext.Items["Language"] as string ?? "eng";
        ViewBag.pageData = new SelectData(search, ViewBag.Language);

        return View();
    }

    [HttpGet("/user-filter/")]
    public async Task<IActionResult> GetByFilterToPopular([FromQuery] string name, int curPage = 1)
    {
        var filter = new ParametersFilter()
        {
            DirectionSort = "desk",
            DateCreateAt = DateCreateRange.None,
            SortBy = "names",
            Name = name,
            Pagination = new PaginationRequest() { Page = curPage, PageSize = 20 },
            UserFilter = new UserFilter()
        };
        var result = await _filterService.GetUsersByFilter(filter);

        if (result == null)
            return StatusCode(500, "Ошибка: результат фильтрации — null");

        if (result.Data == null)
            return StatusCode(500, "Ошибка: Data в результате — null");

        return Json(result.Data); // или return Ok(myFilter); если хочешь явно HTTP 200
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

    [HttpGet("/Resourse/About")]
    public async Task<IActionResult> About()
    {
        var about = await _dataService.GetByPageAsync("About");

        ViewBag.Language = HttpContext.Items["Language"] as string ?? "eng";
        ViewBag.pageData = new SelectData(about, ViewBag.Language);
        var result = await _dataService.AboutEditViewsAsync();
        // Здесь будет логика управления сообществами
        return View("About", result.Data);
    }

    [HttpPut("{id}/role")]
    public async Task<IActionResult> ChangeUserRole([FromRoute] long id, [FromBody] string newRole)
    {
        var allowedRoles = new[] { "User", "Admin" };
        if (string.IsNullOrWhiteSpace(newRole) || !allowedRoles.Contains(newRole, StringComparer.OrdinalIgnoreCase))
        {
            return BadRequest(ApiResponse.CreateFailure("Недопустимая роль"));
        }

        var result = await _userService.ChangeUserRoleAsync(id, newRole);
        return result.IsSuccess
            ? Ok(ApiResponse.CreateSuccess(true))
            : StatusCode(result.Error.StatusCode, ApiResponse.CreateFailure(result.Error.ErrorMessage));
    }

    [HttpGet("/user-admin-list/")]
    public async Task<IActionResult> GetUsersAdmin([FromQuery] string name, [FromQuery] int curPage = 1)
    {
        var users = await _userService.GetUsersAdminAsync(name, curPage);
        return Json(users);
    }
}