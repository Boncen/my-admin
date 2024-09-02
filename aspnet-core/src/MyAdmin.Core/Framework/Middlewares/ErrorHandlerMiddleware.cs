using System.Buffers;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using MyAdmin.Core.Conf;
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
            var result = new ApiResult { Error = error?.Message, ErrCode = MaErrorCode.UnexpectError };
            switch (error)
            {
                case FriendlyException:
                    result.Error = "服务端错误"; // todo Localization
                    break;

            }
            var response = context.Response;
            if (!response.HasStarted)
            {
                response.StatusCode = StatusCodes.Status500InternalServerError;
                response.ContentType = "application/json";
                var resultStr = JsonSerializer.Serialize(result);
                var mem = new ReadOnlyMemory<byte>(Encoding.UTF8.GetBytes(resultStr));
                await response.Body.WriteAsync(mem);
                await response.CompleteAsync();
            }
        }
    }

}
