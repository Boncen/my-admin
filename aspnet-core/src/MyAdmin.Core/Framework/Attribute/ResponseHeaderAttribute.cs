using Microsoft.AspNetCore.Mvc.Filters;

namespace MyAdmin.Core.Framework.Attribute;

public class ResponseHeaderAttribute:ActionFilterAttribute
{
    private readonly string _name;
    private readonly string _value;
    public ResponseHeaderAttribute(string name, string value) => (_name, _value) = (name, value);

    public async override Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        context.HttpContext.Response.Headers.TryAdd(_name, _value);
        await next();
    }
}