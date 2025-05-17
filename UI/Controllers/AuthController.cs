using Application.Utils;
using Core.Entities;
using Core.Interfaces;
using Core.Interfaces.Services;
using Core.Models.Authorisation;
using Core.Models.DataPage;
using Core.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace UI.Controllers;

[Route("Auth")]

public class AuthController : CustomControllerBase
{
    private readonly IAuthService _authService;
    private readonly IDataPageService _dataPage;
    private readonly IErrorMessages _errorMessages;

    public AuthController(IAuthService authService, IDataPageService dataPage, IErrorMessages errorMessages)
    {
        _authService = authService;
        _dataPage = dataPage;
        _errorMessages = errorMessages;
    }

    [HttpGet("login")]

    public async Task<IActionResult> Login()
    {
        var auth = await _dataPage.GetByPageAsync("Authorization");

        //Штука для смены языка, как раз таки мой мидлвеар
        ViewBag.Language = HttpContext.Items["Language"] as string ?? "eng";
        ViewBag.pageData = new SelectData(auth, ViewBag.Language);

        return View("Login");
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login( LoginRequest loginRequest)//не делать асинк!
    {
        var auth = _dataPage.GetByPageAsync("Authorization");

        //Штука для смены языка, как раз таки мой мидлвеар
        ViewBag.Language = HttpContext.Items["Language"] as string ?? "eng";
        ViewBag.pageData = new SelectData(auth.Result, ViewBag.Language);

        if (ModelState.IsValid == false)
        {
            ViewBag.Errors = ModelState
                .Where(ms => ms.Value.Errors.Count > 0)
                .ToDictionary(
                    kvp => kvp.Key, // имя поля модели
                    kvp => _errorMessages.GetMessage(kvp.Value.Errors.First().ErrorMessage, ViewBag.Language)
            );

            return View(loginRequest);
        }

        var result = await _authService.AuthenticateAsync(loginRequest);

        if (result.IsSuccess == false)
        {
            ModelState.AddModelError("UserNotExists", "UserNotExists");
            ViewBag.Errors = ModelState
                .Where(ms => ms.Value.Errors.Count > 0)
                .ToDictionary(
                    kvp => kvp.Key, // имя поля модели
                    kvp => _errorMessages.GetMessage(kvp.Value.Errors.First().ErrorMessage, ViewBag.Language)
                );
            //ViewBag.Errors.Add("Dublicate", "Это дополнительная ошибка");
            return View();
        }

        Response.Cookies.Append("jwt_token", result.Data, new CookieOptions
        {
            HttpOnly = true,
            Secure = true, // включи только если HTTPS
            SameSite = SameSiteMode.Strict,
            Expires = DateTimeOffset.UtcNow.AddHours(1)
        });

        return Redirect($"/Home/{UserId}");//страница на которую перейти
        
    }

    [HttpGet("registration")]

    public async Task<IActionResult> Registration()
{
        var regs = await _dataPage.GetByPageAsync("Registration");

        //Штука для смены языка, как раз таки мой мидлвеар
        ViewBag.Language = HttpContext.Items["Language"] as string ?? "eng";
        ViewBag.pageData = new SelectData(regs, ViewBag.Language);

    // Передаем пустую модель для заполнения формы
    return View("Registration", new RegisterRequest());
}


    [HttpPost("registration")]
    public async Task<IActionResult> Registration( RegisterRequest registerRequest)
    {
        var regs = await _dataPage.GetByPageAsync("Registration");

        //Штука для смены языка, как раз таки мой мидлвеар
        ViewBag.Language = HttpContext.Items["Language"] as string ?? "eng";
        ViewBag.pageData = new SelectData(regs, ViewBag.Language);

        if (ModelState.IsValid == false)
        {
            ViewBag.Errors = ModelState
                .Where(ms => ms.Value.Errors.Count > 0)
                .ToDictionary(
                    kvp => kvp.Key, // имя поля модели
                    kvp => _errorMessages.GetMessage(kvp.Value.Errors.First().ErrorMessage, ViewBag.Language)
                );

            return View(registerRequest);
        }

        var result = await _authService.RegisterAsync(registerRequest);

        if (!result.IsSuccess)
        {
            ModelState.AddModelError("Duplicate", "Duplicate");
            ViewBag.Errors = ModelState
                .Where(ms => ms.Value.Errors.Count > 0)
                .ToDictionary(
                    kvp => kvp.Key, // имя поля модели
                    kvp => _errorMessages.GetMessage(kvp.Value.Errors.First().ErrorMessage, ViewBag.Language)
                );
            return View(registerRequest);
        }

        Response.Cookies.Append("jwt_token", result.Data, new CookieOptions
        {
            HttpOnly = true,
            Secure = true, // включи только если HTTPS
            SameSite = SameSiteMode.Strict,
            Expires = DateTimeOffset.UtcNow.AddHours(1)
        });

        return Redirect($"/Home/{UserId}");
    }

    [Authorize]
    [HttpPatch("changePassword")]
    public async Task<IActionResult> ChangePassword( PasswordUpdateRequest changePasswordRequest)
    {
        if (UserId == null)
            StatusCode(400, ApiResponse.CreateFailure("Ошибка токена"));

        if (ModelState.IsValid == false)
        {
            ViewBag.Errors = ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage);
            ViewBag.OldPassword = changePasswordRequest.OldPassword;
            ViewBag.NewPassword = changePasswordRequest.NewPassword;
            return View();//FromBody это форма
        }

        var result = await _authService.ChangePasswordAsync(UserId.Value, changePasswordRequest.OldPassword,
    changePasswordRequest.NewPassword);

        if (result.IsSuccess == false)
        {
            ViewBag.OldPassword = changePasswordRequest.OldPassword;
            ViewBag.NewPassword = changePasswordRequest.NewPassword;
            ViewBag.Errors = "Неверный пароль";//result.Message;
            return View();
        }

        return Redirect("Home");//страница на которую перейти
    }
}