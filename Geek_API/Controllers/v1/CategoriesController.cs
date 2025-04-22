using Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Geek_API.Controllers.v1;

[ApiVersion("1.0")]
public class CategoriesController : CustomControllerBase
{
    private readonly ICategoryService _categoryService;

    public CategoriesController(ICategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    [HttpGet]

    public async Task<IActionResult> GetByIdAsync(int id)
    {
        var result = await _categoryService.GetByIdAsync(id);
        return result.IsSuccess
            ? Ok(ApiResponse.CreateSuccess(result.Data))
            : StatusCode(result.Error.StatusCode, ApiResponse.CreateFailure(result.Error.ErrorMessage));
    }

    [HttpGet("{id}")]

    public async Task<IActionResult> GetCategoryAsync()
    {
        var result = await _categoryService.GetCategoryAsync();
        return result.IsSuccess
        ? Ok(ApiResponse.CreateSuccess(result.Data))
        : StatusCode(result.Error.StatusCode, ApiResponse.CreateFailure(result.Error.ErrorMessage));
    }

    [HttpPost]

    public async Task<IActionResult> AddCategoryAsync(string title)
    {
        var result = await _categoryService.AddCategoryAsync(title);
        return result.IsSuccess
        ? Ok(ApiResponse.CreateSuccess(result.Data))
        : StatusCode(result.Error.StatusCode, ApiResponse.CreateFailure(result.Error.ErrorMessage));
    }

    [HttpPut]

    public async Task<IActionResult> UpdateCategoryAsync(string title, int id)
    {
        var result = await _categoryService.UpdateCategoryAsync(title, id);
        return result.IsSuccess
        ? Ok(ApiResponse.CreateSuccess(result.Data))
        : StatusCode(result.Error.StatusCode, ApiResponse.CreateFailure(result.Error.ErrorMessage));
    }
}