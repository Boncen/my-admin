using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using MyAdmin.Core.Conf;
using MyAdmin.Core.Identity;
using MyAdmin.Core.Model;

namespace MyAdmin.Core.Framework.Middlewares;

public class JwtValidateMiddleware
{

    private readonly RequestDelegate _next;
    private readonly TokenBlackList _tokenBlackList;
    // private readonly ILogger _logger;

    public JwtValidateMiddleware(RequestDelegate next, TokenBlackList tokenBlackList)//, ILogger logger)
    {
        _next = next;
        _tokenBlackList = tokenBlackList;
    }

    public async Task Invoke(HttpContext context)
    {
        var tokenHeader = context.Request.Headers["Authorization"];
        var token = tokenHeader.FirstOrDefault();
        if (Check.HasValue(token))
        {
            if (_tokenBlackList.IsBlackToken(token!.Replace("Bearer ", "")))
            {
                var response = context.Response;
                if (!response.HasStarted)
                {
                    var result = new ApiResult { Error = "token无效", ErrCode = MaErrorCode.UnexpectError };
                    response.StatusCode = StatusCodes.Status500InternalServerError;
                    response.ContentType = "application/json";
                    var resultStr = JsonSerializer.Serialize(result);
                    var mem = new ReadOnlyMemory<byte>(Encoding.UTF8.GetBytes(resultStr));
                    await response.Body.WriteAsync(mem);
                    await response.CompleteAsync();
                }
            }
        }
        await _next(context);
    }
}
