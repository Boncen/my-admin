using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using MyAdmin.Core.Model;

namespace MyAdmin.Core.Framework.Filter;

public class ValidationFilter:IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        if (!context.ModelState.IsValid)
        {
            var errors = context.ModelState.Values.SelectMany(v => v.Errors).ToList();
            var result = new ApiResult()
            {
                Status = StatusCodes.Status400BadRequest,
                Title = errors.FirstOrDefault()?.ErrorMessage,
            };
            await context.HttpContext.Response.WriteAsync(JsonSerializer.Serialize(result));
            return;
        }
        await next();
    }

}