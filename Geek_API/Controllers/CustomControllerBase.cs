using Microsoft.AspNetCore.Mvc;

namespace Geek_API.Controllers;

[Route("api/v{version:apiVersion}/[controller]")]
[ApiController]
public class CustomControllerBase : Controller
{
}