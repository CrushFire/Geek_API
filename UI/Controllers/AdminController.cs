using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace UI.Controllers;

[Authorize(Roles = "Admin")]
[Route("Admin")]
public class AdminController : Controller
{
    [HttpGet("Panel")]
    public IActionResult AdminPanel()
    {
        ViewBag.Language = HttpContext.Items["Language"] as string ?? "eng";
        return View("AdminPanel");
    }

    [HttpGet("Posts")]
    public IActionResult Posts()
    {
        // Здесь будет логика управления постами
        return View("Post");
    }

    [HttpGet("Categories")]
    public IActionResult Categories()
    {
        // Здесь будет логика управления категориями
        return View("Category");
    }

    [HttpGet("Users")]
    public IActionResult Users()
    {
        // Здесь будет логика управления пользователями
        return View("User");
    }

    [HttpGet("Comments")]
    public IActionResult Comments()
    {
        // Здесь будет логика управления комментариями
        return View("Comment");
    }

    [HttpGet("Communities")]
    public IActionResult Communities()
    {
        // Здесь будет логика управления сообществами
        return View("Community");
    }
}