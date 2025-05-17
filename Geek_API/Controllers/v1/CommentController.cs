using Core.Interfaces.Services;
using Core.Models.Comment;
using Core.Results;
using Microsoft.AspNetCore.Mvc;

namespace Geek_API.Controllers.v1;

[ApiVersion("1.0")]
public class CommentController : CustomControllerBase
{
    private readonly ICommentService _commentService;

    public CommentController(ICommentService commentService)
    {
        _commentService = commentService;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetByIdAsync(long id)
    {
        var result = await _commentService.GetByIdAsync(id);
        return result.IsSuccess
            ? Ok(ApiResponse.CreateSuccess(result.Data))
            : StatusCode(result.Error.StatusCode, ApiResponse.CreateFailure(result.Error.ErrorMessage));
    }

    [HttpGet("byUser")]
    public async Task<IActionResult> GetByUserAsync([FromQuery] long userId, int page = 1, int pageSize = 10)
    {
        if (page < 1 || pageSize < 1)
            StatusCode(400, ApiResponse.CreateFailure("Ошибка параметров page или pageSize"));

        var result = await _commentService.GetByUserIdAsync(userId, page, pageSize);
        return result.IsSuccess
            ? Ok(ApiResponse.CreateSuccess(result.Data))
            : StatusCode(result.Error.StatusCode, ApiResponse.CreateFailure(result.Error.ErrorMessage));
    }

    [HttpGet("byPost")]
    public async Task<IActionResult> GetByPostAsync([FromQuery] long postId, int page = 1, int pageSize = 10)
    {
        if (page < 1 || pageSize < 1)
            StatusCode(400, ApiResponse.CreateFailure("Ошибка параметров page или pageSize"));

        var result = await _commentService.GetByPostIdAsync(postId, page, pageSize);
        return result.IsSuccess
            ? Ok(ApiResponse.CreateSuccess(result.Data))
            : StatusCode(result.Error.StatusCode, ApiResponse.CreateFailure(result.Error.ErrorMessage));
    }

    [HttpPost]
    public async Task<IActionResult> AddAsync([FromBody] CommentAddRequest request)
    {
        if (UserId == null)
            return StatusCode(400, ApiResponse.CreateFailure("Ошибка токена"));

        var result = await _commentService.AddAsync(request, UserId.Value);
        return result.IsSuccess
            ? Ok(ApiResponse.CreateSuccess(result.Data))
            : StatusCode(result.Error.StatusCode, ApiResponse.CreateFailure(result.Error.ErrorMessage));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateAsync([FromRoute] long id, [FromBody] string content)
    {
        if (UserId == null)
            return StatusCode(400, ApiResponse.CreateFailure("Ошибка токена"));

        var result = await _commentService.UpdateAsync(id, content, UserId.Value);
        return result.IsSuccess
            ? NoContent()
            : StatusCode(result.Error.StatusCode, ApiResponse.CreateFailure(result.Error.ErrorMessage));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAsync([FromRoute] long id)
    {
        if (UserId == null)
            return StatusCode(400, ApiResponse.CreateFailure("Ошибка токена"));

        var result = await _commentService.DeleteAsync(id, UserId.Value);
        return result.IsSuccess
            ? NoContent()
            : StatusCode(result.Error.StatusCode, ApiResponse.CreateFailure(result.Error.ErrorMessage));
    }
}