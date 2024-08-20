using System.Text.Json;
using Microsoft.AspNetCore.Http;
using MyAdmin.Core.Exception;
using MyAdmin.Core.Logger;
using MyAdmin.Core.Model;

namespace MyAdmin.Core.Framework.Middlewares;

public class ErrorHandlerMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger _logger;

    public ErrorHandlerMiddleware(RequestDelegate next, ILogger logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (System.Exception error)
        {
            _logger.LogError(error);
            var result = new ApiResult { Title = error?.Message, Status = StatusCodes.Status500InternalServerError };
            switch (error)
            {
                case FriendlyException:
                    result.Title = "服务端错误"; // todo Localization
                    break;

            }
            var response = context.Response;
            response.ContentType = "application/json";
            var rsp = JsonSerializer.Serialize(result);
            await response.WriteAsync(rsp);
        }
    }

}
