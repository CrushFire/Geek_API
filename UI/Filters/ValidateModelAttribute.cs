using Core.Results;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Geek_API.Filters;

public class ValidateModelAttribute : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        if (!context.ModelState.IsValid)
        {
            var errors = context.ModelState
                .Where(x => x.Value.Errors.Any())
                .ToDictionary(
                    x => x.Key,
                    x => string.Join("; ", x.Value.Errors.Select(e => e.ErrorMessage))
                );

            var response = ApiResponse.CreateFailure("Неверный данные", errors);

            context.Result = new BadRequestObjectResult(response);
        }
    }
}