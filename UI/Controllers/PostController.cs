using Application.Utils;
using Core.Entities;
using Core.Enums;
using Core.Interfaces.Services;
using Core.Models;
using Core.Models.Filter;
using Core.Models.Post;
using Core.Results;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace UI.Controllers;

[Route("Post")]

public class PostsController : CustomControllerBase
{
    private readonly IPostService _postService;
    private readonly IFilterService _filterService;
    private readonly IImageService _imageService;

    public PostsController(IPostService postService, IFilterService filterService, IImageService imageService)
    {
        _filterService = filterService;
        _postService = postService;
        _imageService = imageService;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(long id)
    {
        ViewBag.userTokenId = UserId.Value;
        var result = await _postService.GetByIdAsync(id);

        return View("Post", result.Data);
    }

    [HttpGet("/by-community/")]
    public async Task<IActionResult> GetByCommunityIdAsync([FromQuery] long communityId, int curPage)
    {
        var pagination = new PaginationRequest()
        {
            Page = curPage,
            PageSize = 20
        };

        var result = await _postService.GetByCommunityIdAsync(communityId, pagination);

        return Json(result.Data);
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

    [HttpGet("/by-filter/popular/")]
    public async Task<IActionResult> GetByFilterToPopular([FromQuery] int curPage = 1)
    {
        var filter = new ParametersFilter()
        {
            DirectionSort = "desk",
            DateCreateAt = DateCreateRange.Week,
            SortBy = "likes",
            Pagination = new PaginationRequest() { Page = curPage, PageSize = 20 },
            PostFilter = new PostFilter()
        };
        var result = await _filterService.GetPostsByFilter(filter);

        //Штука для смены языка, как раз таки мой мидлвеар
        //ViewBag.Language = HttpContext.Items["Language"] as string ?? "eng";
        //ViewBag.pageData = new SelectData(auth, ViewBag.Language);

        if (result == null)
            return StatusCode(500, "Ошибка: результат фильтрации — null");

        if (result.Data == null)
            return StatusCode(500, "Ошибка: Data в результате — null");

        return Json(result.Data); // или return Ok(myFilter); если хочешь явно HTTP 200
    }

    [HttpGet("/by-filter/new/")]
    public async Task<IActionResult> GetByFilterToNew([FromQuery] int curPage = 1)
    {
        var filter = new ParametersFilter()
        {
            DirectionSort = "desk",
            DateCreateAt = DateCreateRange.Week,
            SortBy = "created",
            Pagination = new PaginationRequest() { Page = curPage, PageSize = 20 },
            PostFilter = new PostFilter()
        };
        var result = await _filterService.GetPostsByFilter(filter);

        //Штука для смены языка, как раз таки мой мидлвеар
        //ViewBag.Language = HttpContext.Items["Language"] as string ?? "eng";
        //ViewBag.pageData = new SelectData(auth, ViewBag.Language);

        if (result == null)
            return StatusCode(500, "Ошибка: результат фильтрации — null");

        if (result.Data == null)
            return StatusCode(500, "Ошибка: Data в результате — null");

        return Json(result.Data); // или return Ok(myFilter); если хочешь явно HTTP 200
    }

    [HttpPost("/increment-view/{postId}")]
    public async Task<IActionResult> IncrementView([FromRoute] int postId)
    {
        var result = await _postService.HasBeenSeen(postId);

        if(result == null)
        {
            return NotFound("Такого поста нет");
        }

        return Ok(result.Data);
    }

    [HttpPost("/update-reactions")]
    public async Task<IActionResult> UpdateReactions([FromBody] PostReaction reaction)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var result = await _postService.PostReactionsAsync(reaction);

        if(result == null)
        {
            return NotFound("Таких реакций к посту нет");
        }

        return Ok(result.Data);
    }

    [HttpGet("/liked-posts/")]
    public async Task<IActionResult> TakeUserLikesPost([FromQuery] long userId, int curPage = 1)
    {
        var pagination = new PaginationRequest()
        {
            Page = curPage,
            PageSize = 10
        };
        var result = await _postService.GetUserLikesPost(pagination, userId);

        //Штука для смены языка, как раз таки мой мидлвеар
        //ViewBag.Language = HttpContext.Items["Language"] as string ?? "eng";
        //ViewBag.pageData = new SelectData(auth, ViewBag.Language);

        return Json(result.Data); // или return Ok(myFilter); если хочешь явно HTTP 200
    }

    [HttpGet("Create")]

    public IActionResult Create()
    {
        ViewBag.Language = HttpContext.Items["Language"] as string ?? "eng";
        ViewBag.UserId = UserId.Value;

        return View();
    }

    [HttpGet("Search")]
    public IActionResult Search()
    {
        ViewBag.Language = HttpContext.Items["Language"] as string ?? "eng";
        ViewBag.UserId = UserId.Value;

        return View();
    }

    [HttpGet("/post-filter/")]
    public async Task<IActionResult> GetByFilterToPopular([FromQuery] string name, int curPage = 1)
    {
        var filter = new ParametersFilter()
        {
            DirectionSort = "desk",
            DateCreateAt = DateCreateRange.None,
            SortBy = "likes",
            Name = name,
            Pagination = new PaginationRequest() { Page = curPage, PageSize = 20 },
            PostFilter = new PostFilter()
        };
        var result = await _filterService.GetPostsByFilter(filter);

        //Штука для смены языка, как раз таки мой мидлвеар
        //ViewBag.Language = HttpContext.Items["Language"] as string ?? "eng";
        //ViewBag.pageData = new SelectData(auth, ViewBag.Language);

        if (result == null)
            return StatusCode(500, "Ошибка: результат фильтрации — null");

        if (result.Data == null)
            return StatusCode(500, "Ошибка: Data в результате — null");

        return Json(result.Data); // или return Ok(myFilter); если хочешь явно HTTP 200
    }

    [HttpGet("/published-posts/")]
    public async Task<IActionResult> TakePostPublishUser([FromQuery] long userId, int curPage = 1)
    {
        var pagination = new PaginationRequest()
        {
            Page = curPage,
            PageSize = 10
        };
        var result = await _postService.GetPostPublishUser(pagination, userId);

        //Штука для смены языка, как раз таки мой мидлвеар
        //ViewBag.Language = HttpContext.Items["Language"] as string ?? "eng";
        //ViewBag.pageData = new SelectData(auth, ViewBag.Language);

        return Json(result.Data); // или return Ok(myFilter); если хочешь явно HTTP 200
    }


    [HttpPost("/create-post/")]
    public async Task<IActionResult> AddAsync([FromForm] PostAddRequest request)
    {
        if (UserId == null)
            return StatusCode(400, ApiResponse.CreateFailure("Ошибка токена"));

        var result = await _postService.AddAsync(request, UserId.Value);

        return Json(result.Data);
    }

    [HttpGet("Edit/{id}")]
    public async Task<IActionResult> Edit(long id)
    {
        ViewBag.Language = HttpContext.Items["Language"] as string ?? "eng";
        ViewBag.UserId = UserId.Value;
        
        return View();
    }

    [HttpPost("/edit-post/")]
    public async Task<IActionResult> Edit([FromQuery] long id, [FromForm] PostUpdateRequest request)
    {
        if (UserId == null)
            return StatusCode(400, ApiResponse.CreateFailure("Ошибка токена"));

        var result = await _postService.UpdateAsync(id, request, UserId.Value);

        return Json(result.Data);
    }

    [HttpGet("/get-post/")]
    public async Task<IActionResult> GetPost([FromQuery] long postId)
    {
        ViewBag.userTokenId = UserId.Value;
        var result = await _postService.GetByIdWithCategoriesAsync(postId);

        return Json(result.Data);
    }

    [HttpDelete("/remove-post-image/")]
    public async Task<IActionResult> RemoveImageFromPost([FromQuery] long imgId)
    {
        var result = await _imageService.RemoveImageFromServer(imgId);

        return Json(result);
    }

    [HttpDelete("Delete")]
    public async Task<IActionResult> Delete([FromQuery] long postId)
    {
        if (UserId == null)
            return StatusCode(400, ApiResponse.CreateFailure("Ошибка токена"));

        var result = await _postService.DeleteAsync(postId, UserId.Value);

        return Json(result.Data);
    }
}