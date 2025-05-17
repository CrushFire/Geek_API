using Core.Entities;
using Microsoft.AspNetCore.Mvc;


namespace UI.Controllers
{
    public class SetCookiesController : CustomControllerBase
    {
        [HttpGet]

        public void SetCookie(string language)
        {
            if (string.IsNullOrEmpty(language))
            {
                language = "ru"; // например, язык по умолчанию
            }
            // Установка cookie с языком (на 30 дней)
            Response.Cookies.Append("lang", language, new CookieOptions
            {
                Expires = DateTimeOffset.UtcNow.AddDays(30),
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Lax
            });
        }

        [HttpGet]

        public void SetLanguage()
        {
            ViewBag.Language = Request.Cookies["language"] ?? "ru";
            // Пробуем взять язык из query string
            string lang = HttpContext.Request.Query["lang"];

            // Если не пришел – пробуем из куки
            if (string.IsNullOrEmpty(lang))
            {
                lang = Request.Cookies["Language"];
            }

            ViewBag.Language = lang ?? "eng";
        }
    }
}
