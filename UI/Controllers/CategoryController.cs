using Application.Services;
using Core.Interfaces.Services;
using Core.Models;
using Core.Models.Category;
using Core.Results;
using Microsoft.AspNetCore.Mvc;

namespace UI.Controllers;

[Route("Category")]

public class CategoryController : CustomControllerBase
{
    private readonly ICategoryService _categoryService;

    public CategoryController(ICategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    [HttpGet("{id}")]

    public async Task<IActionResult> GetByIdAsync([FromRoute] int id)
    {
        var result = await _categoryService.GetByIdAsync(id);
        if (ModelState.IsValid == false)
        {
            ViewBag.Errors = ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage);
        }
        return result.IsSuccess
            ? Ok(ApiResponse.CreateSuccess(result.Data))
            : StatusCode(result.Error.StatusCode, ApiResponse.CreateFailure(result.Error.ErrorMessage));
    }

    [HttpGet]

    public async Task<IActionResult> GetCategoryAsync([FromQuery] int curPage)
    {
        var pagination = new PaginationRequest
        {
            Page = curPage,
            PageSize = 20
        };
        var result = await _categoryService.GetCategoryAsync(pagination);

        return Json(result.Data);
    }

    [HttpPost]

    public async Task<IActionResult> AddCategoryAsync([FromBody] CategoryRequest categoryRequest)
    {
        var result = await _categoryService.AddCategoryAsync(categoryRequest);
        return result.IsSuccess
        ? Ok(ApiResponse.CreateSuccess(result.Data))
        : StatusCode(result.Error.StatusCode, ApiResponse.CreateFailure(result.Error.ErrorMessage));
    }

    [HttpPut]

    public async Task<IActionResult> UpdateCategoryAsync([FromBody] CategoryRequest categoryRequest, [FromQuery] int id)
    {
        var result = await _categoryService.UpdateCategoryAsync(categoryRequest, id);
        return result.IsSuccess
        ? Ok(ApiResponse.CreateSuccess(result.Data))
        : StatusCode(result.Error.StatusCode, ApiResponse.CreateFailure(result.Error.ErrorMessage));
    }
}