using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using MyAdmin.Core.Model;
using MyAdmin.Core.Utilities;

namespace MyAdmin.Core.Framework.Attribute;

public class ModelValidateAttribute:ActionFilterAttribute
{
    public async override Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        // if (context.ModelState.ErrorCount > 0 && !context.ModelState.IsValid)
        // {
        //     var errors = context.ModelState.Values.SelectMany(x => x.Errors).ToList();
        //     var result = new ApiResult()
        //     {
        //         Code = StatusCodes.Status400BadRequest,
        //         Msg = errors.FirstOrDefault()?.ErrorMessage,
        //         Success = false
        //     };
        //     await context.HttpContext.Response.WriteAsync(result.ToJsonString());
        //     return;
        // }
        //
        await base.OnActionExecutionAsync(context, next);
    }
}