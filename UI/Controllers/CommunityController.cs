using Application.Services;
using Application.Utils;
using Core.Entities;
using Core.Enums;
using Core.Interfaces.Services;
using Core.Models;
using Core.Models.Community;
using Core.Models.Filter;
using Core.Models.Post;
using Core.Results;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace UI.Controllers;

[Route("Community")]

public class CommunityController : CustomControllerBase
{
    private readonly ICommunityService _communityService;
    private readonly IFilterService _filterService;
    private readonly IDataPageService _dataService;

    public CommunityController(ICommunityService communityService, IFilterService filterService, IDataPageService dataService)
    {
        _communityService = communityService;
        _filterService = filterService;
        _dataService = dataService;
    }

    [HttpGet]

    public async Task<IActionResult> GetCommunityAsync([FromQuery] int page = 1, int pageSize = 10)
    {
        var result = await _communityService.GetCommunityAsync(page, pageSize);

        return result.IsSuccess
            ? Ok(ApiResponse.CreateSuccess(result.Data))
            : StatusCode(result.Error.StatusCode, ApiResponse.CreateFailure(result.Error.ErrorMessage));
    }

    [HttpGet("Create")]

    public async Task<IActionResult> Create()
    {
        var cre = await _dataService.GetByPageAsync("CreateCommunity");

        ViewBag.Language = HttpContext.Items["Language"] as string ?? "eng";
        ViewBag.pageData = new SelectData(cre, ViewBag.Language);

        return View();
    }

    [HttpGet("Search")]

    public IActionResult Search()
    {
        ViewBag.Language = HttpContext.Items["Language"] as string ?? "eng";
        ViewBag.UserId = UserId.Value;

        return View();
    }

    [HttpGet("/community-filter/")]
    public async Task<IActionResult> GetByFilterToPopular([FromQuery] string name, int curPage = 1)
    {
        var filter = new ParametersFilter()
        {
            DirectionSort = "desk",
            DateCreateAt = DateCreateRange.None,
            SortBy = "subscribers",
            Name = name,
            Pagination = new PaginationRequest() { Page = curPage, PageSize = 20 },
            CommunityFilter = new CommunityFilter()
        };
        var result = await _filterService.GetCommunitiesByFilter(filter);

        if (result == null)
            return StatusCode(500, "Ошибка: результат фильтрации — null");

        if (result.Data == null)
            return StatusCode(500, "Ошибка: Data в результате — null");

        return Json(result.Data); // или return Ok(myFilter); если хочешь явно HTTP 200
    }

    [HttpGet("{id}")]

    public async Task<IActionResult> GetByIdAsync(long id)
    {
        var result = await _communityService.GetByIdAsync(id);

        ViewBag.userIdToken = UserId.Value;

        return View("Community", result.Data);
    }

    [HttpGet("/subscribed-communities/")]

    public async Task<IActionResult> GetCommunitiesSubscribeUser([FromQuery] long userId, int curPage)
    {
        var pagination = new PaginationRequest()
        {
            Page = curPage,
            PageSize = 10
        };

        var result = await _communityService.GetCommunitiesSubscribeUser(pagination, userId);

        return Json(result.Data);
    }

    [HttpGet("/created-communities/")]

    public async Task<IActionResult> GetCommunitiesCreatedUser([FromQuery] long userId, int curPage)
    {
        var pagination = new PaginationRequest()
        {
            Page = curPage,
            PageSize = 10
        };

        var result = await _communityService.GetCommunitiesCreatedUser(pagination, userId);

        return Json(result.Data);
    }

    [HttpGet("Explore")]
    public async Task<IActionResult> Explore([FromQuery] int categoryId)
    {
        ViewBag.Language = HttpContext.Items["Language"] as string ?? "eng";
        ViewBag.UserId = UserId.Value;

        return View();
    }

    [HttpGet("/community-explore/")]
    public async Task<IActionResult> CommunityExplore([FromQuery] int categoryId)
    {
        var pagination = new PaginationRequest()
        {
            Page = 1,
            PageSize = 10,
        };

        var result = await _communityService.CommunityExplore(categoryId, pagination);

        return Json(result.Data);
    }

    [HttpGet("/check-role-community/")]

    public async Task<IActionResult> SubOrNo([FromQuery] long userId, [FromQuery] long communityId)
    {
        var result = await _communityService.SubOrNo(userId, communityId);

        return Json(result.Data);
    }

    [HttpPost("/create-community")]

    public async Task<IActionResult> AddCommunityAsync([FromForm] CommunityAddRequest communityAddRequest)
    {
        var result = await _communityService.AddCommunityAsync(communityAddRequest, UserId.Value);

        return Json(result.Data);
    }

    [HttpPost("/sub")]

    public async Task<IActionResult> SubscribeAsync([FromBody] long communityId, [FromQuery] long userId)
    {
        var result = await _communityService.SubsribeAsync(userId, communityId);

        return Json(result.Data);
    }

    [HttpDelete("/unsub")]

    public async Task<IActionResult> UnSubscribeAsync([FromBody] long communityId, [FromQuery] long userId)
    {
        var result = await _communityService.UnSubscribeAsync(userId, communityId);

        return Json(result.Data);
    }

    [HttpGet("Edit/{id}")]
    public async Task<IActionResult> Edit(long id)
    {
        ViewBag.Language = HttpContext.Items["Language"] as string ?? "eng";
        ViewBag.UserId = UserId.Value;

        return View();
    }

    [HttpPost("/edit-community/")]
    public async Task<IActionResult> Edit([FromQuery] long id, [FromForm] CommunityUpdateRequest request)
    {
        if (UserId == null)
            return StatusCode(400, ApiResponse.CreateFailure("Ошибка токена"));

        var result = await _communityService.UpdateCommunityAsync(request, id);

        return Json(result.Data);
    }

    [HttpGet("/get-community/")]
    public async Task<IActionResult> GetPost([FromQuery] long communityId)
    {
        ViewBag.userTokenId = UserId.Value;
        var result = await _communityService.GetByIdWithCategoriesAsync(communityId);

        return Json(result.Data);
    }

    [HttpDelete("Delete")]

    public async Task<IActionResult> Delete([FromQuery] long communityId)
    {
        var result = await _communityService.DeleteCommunityAsync(communityId);

        return Json(result.Data);
    }
}