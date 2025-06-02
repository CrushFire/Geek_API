using Application.Services;
using Core.Interfaces.Services;
using Core.Models;
using Core.Models.Community;
using Core.Results;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace UI.Controllers;

[Route("Community")]

public class CommunityController : CustomControllerBase
{
    private readonly ICommunityService _communityService;

    public CommunityController(ICommunityService communityService)
    {
        _communityService = communityService;
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

    public IActionResult Create()
    {
        ViewBag.Language = HttpContext.Items["Language"] as string ?? "eng";

        return View();
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

    [HttpPut]

    public async Task<IActionResult> UpdateCommunityAsync([FromBody] CommunityAddRequest communityAddRequest, [FromRoute] long id)
    {
        var result = await _communityService.UpdateCommunityAsync(communityAddRequest, id);

        return result.IsSuccess
            ? Ok(ApiResponse.CreateSuccess(result.Data))
            : StatusCode(result.Error.StatusCode, ApiResponse.CreateFailure(result.Error.ErrorMessage));
    }

    [HttpDelete]

    public async Task<IActionResult> DeleteCommunityAsync([FromRoute] long id)
    {
        var result = await _communityService.DeleteCommunityAsync(id);

        return result.IsSuccess
            ? Ok(ApiResponse.CreateSuccess(result.Data))
            : StatusCode(result.Error.StatusCode, ApiResponse.CreateFailure(result.Error.ErrorMessage));
    }
}