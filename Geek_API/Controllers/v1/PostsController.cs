using Core.Interfaces.Services;
using Core.Models.Post;
using Microsoft.AspNetCore.Mvc;

namespace Geek_API.Controllers.v1;

[ApiVersion("1.0")]
public class PostsController : CustomControllerBase
{
    private readonly IPostService _postService;

    public PostsController(IPostService postService)
    {
        _postService = postService;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetByIdAsync(long id)
    {
        var result = await _postService.GetByIdAsync(id);
        return result.IsSuccess
            ? Ok(ApiResponse.CreateSuccess(result.Data))
            : StatusCode(result.Error.StatusCode, ApiResponse.CreateFailure(result.Error.ErrorMessage));
    }

    //TODO 
    //Объеденил бы в один общий с блекджеком и шлюхами
    [HttpGet("byCommunity")]
    public async Task<IActionResult> GetByCommunityIdAsync([FromQuery] long communityId, int page = 1,
        int pageSize = 10)
    {
        if (page < 1 || pageSize < 1)
            StatusCode(400, ApiResponse.CreateFailure("Ошибка параметров page или pageSize"));

        var result = await _postService.GetByCommunityIdAsync(communityId, page, pageSize);
        return result.IsSuccess
            ? Ok(ApiResponse.CreateSuccess(result.Data))
            : StatusCode(result.Error.StatusCode, ApiResponse.CreateFailure(result.Error.ErrorMessage));
    }

    [HttpGet("byUserId")]
    public async Task<IActionResult> GetByUserIdAsync([FromQuery] long userId, int page = 1, int pageSize = 10)
    {
        if (page < 1 || pageSize < 1)
            StatusCode(400, ApiResponse.CreateFailure("Ошибка параметров page или pageSize"));

        var result = await _postService.GetByUserIdAsync(userId, page, pageSize);
        return result.IsSuccess
            ? Ok(ApiResponse.CreateSuccess(result.Data))
            : StatusCode(result.Error.StatusCode, ApiResponse.CreateFailure(result.Error.ErrorMessage));
    }

    [HttpPost]
    public async Task<IActionResult> AddAsync([FromBody] PostAddRequest request)
    {
        if (UserId == null)
            return StatusCode(400, ApiResponse.CreateFailure("Ошибка токена"));

        var result = await _postService.AddAsync(request, UserId.Value);
        return result.IsSuccess
            ? Ok(ApiResponse.CreateSuccess(result.Data))
            : StatusCode(result.Error.StatusCode, ApiResponse.CreateFailure(result.Error.ErrorMessage));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateAsync([FromRoute] long id, [FromBody] PostUpdateRequest request)
    {
        if (UserId == null)
            return StatusCode(400, ApiResponse.CreateFailure("Ошибка токена"));

        var result = await _postService.UpdateAsync(id, request, UserId.Value);
        return result.IsSuccess
            ? NoContent()
            : StatusCode(result.Error.StatusCode, ApiResponse.CreateFailure(result.Error.ErrorMessage));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAsync([FromRoute] long id)
    {
        if (UserId == null)
            return StatusCode(400, ApiResponse.CreateFailure("Ошибка токена"));

        var result = await _postService.DeleteAsync(id, UserId.Value);
        return result.IsSuccess
            ? NoContent()
            : StatusCode(result.Error.StatusCode, ApiResponse.CreateFailure(result.Error.ErrorMessage));
    }
}