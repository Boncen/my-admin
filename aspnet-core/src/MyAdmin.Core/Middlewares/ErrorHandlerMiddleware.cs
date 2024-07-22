using System.Text.Json;
using Microsoft.AspNetCore.Http;
using MyAdmin.Core;

namespace MyAdmin.Core.Middleware;

public class ErrorHandlerMiddleware
{
    private readonly RequestDelegate _next;

    public ErrorHandlerMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (System.Exception error)
        {
            // todo log
            var result = new ApiResult { Msg = error?.Message, Code = StatusCodes.Status500InternalServerError };
            switch (error)
            {
                case FriendlyException:
                    result.Msg = "服务端错误"; // todo Localization
                    break;

            }
            var response = context.Response;
            response.ContentType = "application/json";
            var rsp = JsonSerializer.Serialize(result);
            await response.WriteAsync(rsp);
        }
    }

}
