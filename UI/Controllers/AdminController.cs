using Application.Utils;
using Core.Entities;
using Core.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace UI.Controllers;

[Authorize(Roles = "Admin")]
[Route("Admin")]
public class AdminController : Controller
{
    private readonly IDataPageService _dataService;

    public AdminController(IDataPageService dataService)
    {
        _dataService = dataService;
    }

    [HttpGet("Panel")]
    public async Task<IActionResult> AdminPanel()
    {
        var admin = await _dataService.GetByPageAsync("Admin");

        ViewBag.Language = HttpContext.Items["Language"] as string ?? "eng";
        ViewBag.pageData = new SelectData(admin, ViewBag.Language);

        return View("AdminPanel");
    }

    [HttpGet("Posts")]
    public async Task<IActionResult> Posts()
    {
        var admin = await _dataService.GetByPageAsync("Admin");

        ViewBag.Language = HttpContext.Items["Language"] as string ?? "eng";
        ViewBag.pageData = new SelectData(admin, ViewBag.Language);
        // Здесь будет логика управления постами
        return View("Post");
    }

    [HttpGet("Categories")]
    public async Task<IActionResult> Categories()
    {
        var admin = await _dataService.GetByPageAsync("Admin");

        ViewBag.Language = HttpContext.Items["Language"] as string ?? "eng";
        ViewBag.pageData = new SelectData(admin, ViewBag.Language);
        // Здесь будет логика управления категориями
        return View("Category");
    }

    [HttpGet("Users")]
    public async Task<IActionResult> Users()
    {
        var admin = await _dataService.GetByPageAsync("Admin");

        ViewBag.Language = HttpContext.Items["Language"] as string ?? "eng";
        ViewBag.pageData = new SelectData(admin, ViewBag.Language);
        // Здесь будет логика управления пользователями
        return View("User");
    }

    [HttpGet("Comments")]
    public async Task<IActionResult> Comments()
    {
        var admin = await _dataService.GetByPageAsync("Admin");

        ViewBag.Language = HttpContext.Items["Language"] as string ?? "eng";
        ViewBag.pageData = new SelectData(admin, ViewBag.Language);
        // Здесь будет логика управления комментариями
        return View("Comment");
    }

    [HttpGet("Communities")]
    public async Task<IActionResult> Communities()
    {
        var admin = await _dataService.GetByPageAsync("Admin");

        ViewBag.Language = HttpContext.Items["Language"] as string ?? "eng";
        ViewBag.pageData = new SelectData(admin, ViewBag.Language);
        // Здесь будет логика управления сообществами
        return View("Community");
    }

    [HttpGet("/Resourse/About")]
    public async Task<IActionResult> About()
    {
        var admin = await _dataService.GetByPageAsync("Admin");

        ViewBag.Language = HttpContext.Items["Language"] as string ?? "eng";
        ViewBag.pageData = new SelectData(admin, ViewBag.Language);
        // Здесь будет логика управления сообществами
        return View("About");
    }
}