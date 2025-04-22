using Application.Services;
using Core.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace Geek_API.Controllers.v1;

[ApiVersion("1.0")]
public class CategoryController : CustomControllerBase
{
    private readonly ICategoryService _categoryService;

    public CategoryController(ICategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    [HttpGet]

    public async Task<IActionResult> GetByIdAsync([FromRoute] int id)
    {
        var result = await _categoryService.GetByIdAsync(id);
        return result.IsSuccess
            ? Ok(ApiResponse.CreateSuccess(result.Data))
            : StatusCode(result.Error.StatusCode, ApiResponse.CreateFailure(result.Error.ErrorMessage));
    }

    [HttpGet("{id}")]

    public async Task<IActionResult> GetCategoryAsync([FromQuery] int page = 1, int pageSize = 10)
    {
        var result = await _categoryService.GetCategoryAsync(1, 10);
        return result.IsSuccess
        ? Ok(ApiResponse.CreateSuccess(result.Data))
        : StatusCode(result.Error.StatusCode, ApiResponse.CreateFailure(result.Error.ErrorMessage));
    }

    [HttpPost]

    public async Task<IActionResult> AddCategoryAsync([FromBody] string title)
    {
        var result = await _categoryService.AddCategoryAsync(title);
        return result.IsSuccess
        ? Ok(ApiResponse.CreateSuccess(result.Data))
        : StatusCode(result.Error.StatusCode, ApiResponse.CreateFailure(result.Error.ErrorMessage));
    }

    [HttpPut]

    public async Task<IActionResult> UpdateCategoryAsync([FromBody] string title, [FromRoute] int id)
    {
        var result = await _categoryService.UpdateCategoryAsync(title, id);
        return result.IsSuccess
        ? Ok(ApiResponse.CreateSuccess(result.Data))
        : StatusCode(result.Error.StatusCode, ApiResponse.CreateFailure(result.Error.ErrorMessage));
    }
}