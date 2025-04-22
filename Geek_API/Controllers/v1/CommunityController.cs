using Application.Services;
using Core.Interfaces.Services;
using Core.Models.Community;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace Geek_API.Controllers.v1;

[ApiVersion("1.0")]
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

    [HttpGet("{id}")]

    public async Task<IActionResult> GetByIdAsync([FromRoute] long id)
    {
        var result = await _communityService.GetByIdAsync(id);

        return result.IsSuccess
            ? Ok(ApiResponse.CreateSuccess(result.Data))
            : StatusCode(result.Error.StatusCode, ApiResponse.CreateFailure(result.Error.ErrorMessage));
    }

    [HttpGet("byUser")]

    public async Task<IActionResult> GetByUserIdAsync([FromQuery] long userId)
    {
        var result = await _communityService.GetByUserIdAsync(userId);

        return result.IsSuccess
            ? Ok(ApiResponse.CreateSuccess(result.Data))
            : StatusCode(result.Error.StatusCode, ApiResponse.CreateFailure(result.Error.ErrorMessage));
    }

    [HttpPost]

    public async Task<IActionResult> AddCommunityAsync([FromBody] CommunityAddRequest communityAddRequest)
    {
        var result = await _communityService.AddCommunityAsync(communityAddRequest);

        return result.IsSuccess
                ? Ok(ApiResponse.CreateSuccess(result.Data))
                : StatusCode(result.Error.StatusCode, ApiResponse.CreateFailure(result.Error.ErrorMessage));
    }

    [HttpPost("sub")]

    public async Task<IActionResult> SubscribeAsync([FromBody] long userId, long communityId)
    {
        var result = await _communityService.SubsribeAsync(userId, communityId);

        return result.IsSuccess
            ? Ok(ApiResponse.CreateSuccess(result.Data))
            : StatusCode(result.Error.StatusCode, ApiResponse.CreateFailure(result.Error.ErrorMessage));
    }

    [HttpDelete("unsub")]

    public async Task<IActionResult> UnSubscribeAsync([FromBody] long userId, long communityId)
    {
        var result = await _communityService.UnSubscribeAsync(userId, communityId);

        return result.IsSuccess
            ? Ok(ApiResponse.CreateSuccess(result.Data))
            : StatusCode(result.Error.StatusCode, ApiResponse.CreateFailure(result.Error.ErrorMessage));
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