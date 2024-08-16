using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.Logging;
using MyAdmin.Core.Model.BuildIn;
using MyAdmin.Core.Repository;
using MyAdmin.Core.Utilities;
using ILogger = MyAdmin.Core.Logger.ILogger;

namespace MyAdmin.Core.Middlewares;

public class RequestMonitorMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger _logger;

    public RequestMonitorMiddleware(RequestDelegate next, ILogger logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task Invoke(HttpContext context)
    {
        var request = context.Request;
        using var memStream = new MemoryStream();
        await request.Body.CopyToAsync(memStream);
        var bodyContent = memStream.GetStringByPipe();
        memStream.Position = 0;
        request.Body = memStream;
        
        // var headers = request.Headers.ToJsonString();
        var contentType = request.ContentType;
        var host = request.Host.ToString();
        var url = request.GetDisplayUrl();
        var method = request.Method;
        // var path = request.Path;
        // var queryString = request.QueryString.ToString();
        var trace = context.TraceIdentifier;
        
        // Console.WriteLine(new {bodyContent,headers,contentType,host,url,method, path,queryString,trace, connection = connection.ToString()});
        
        // response
        var originalResponseBody = context.Response.Body;
        using var rspMemoryStream = new MemoryStream();
        context.Response.Body = rspMemoryStream;
        rspMemoryStream.Position = 0;
        
        await _next(context);

        var rspBodyContent = rspMemoryStream.GetStringByPipe();
        rspMemoryStream.Position = 0;
        await rspMemoryStream.CopyToAsync(originalResponseBody);
        
        var response = context.Response;
        // var rspHeader = response.Headers.ToJsonString();
        var statusCode = response.StatusCode.ToString();
        // Console.WriteLine(new {connection = connection2.ToString(), trace = context.TraceIdentifier, rspHeader, statusCode,rspBodyContent});
        _logger.Log(new Log()
        {
            Id = Guid.NewGuid(),
            Content = "",
            ContentType = contentType,
            Exceptions = String.Empty,
            Host = host,
            HttpMethod = method,
            IpAddress = "",
            Level = LogLevel.Trace,
            LogTime = DateTime.Now,
            Origin = request.Headers["Origin"],
            Referer = request.Headers["Referer"],
            Url = url,
            Trace = trace,
            RequestBody = bodyContent,
            ResponseStatusCode = statusCode,
            ResponseBody = rspBodyContent,
        });
    }

}
