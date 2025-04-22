using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;

namespace Geek_API.Controllers;

[Route("api/v{version:apiVersion}/[controller]")]
[ApiController]
public class CustomControllerBase : Controller
{
    //Из токена получаем id пользователя, если токена нет, то Null
    protected long? UserId =>
        long.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var userId) ? userId : null;
}