using Microsoft.AspNetCore.Mvc;

namespace UI.Controllers;

public class HomeController : CustomControllerBase
{
    [HttpGet("/")]
    public IActionResult Index()
    {
        if (UserId.HasValue)
        {
            return Redirect($"/popular/{UserId}");
        }
        else
        {
            return Redirect("/auth/login");
        }
    }
}