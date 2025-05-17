using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Utils
{
    public class LanguageMiddleware
    {
        private readonly RequestDelegate _next;

        public LanguageMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Получаем язык из query
            var langFromQuery = context.Request.Query["lang"].ToString();
            var langFromCookie = context.Request.Cookies["Language"];

            var lang = string.IsNullOrEmpty(langFromQuery) ? langFromCookie : langFromQuery;

            // Если передан новый язык — сохраняем в cookie
            if (!string.IsNullOrEmpty(langFromQuery))
            {
                context.Response.Cookies.Append("Language", langFromQuery, new CookieOptions
                {
                    Expires = DateTimeOffset.UtcNow.AddYears(1)
                });
            }
            
            // Устанавливаем язык в context
            context.Items["Language"] = lang ?? "eng";

            await _next(context);
        }
    }

}
