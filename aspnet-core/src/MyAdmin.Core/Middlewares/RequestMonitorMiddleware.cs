using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using MyAdmin.Core.Utilities;

namespace MyAdmin.Core.Middlewares;

public class RequestMonitorMiddleware
{
    private readonly RequestDelegate _next;

    public RequestMonitorMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        var request = context.Request;
        using var memStream = new MemoryStream();
        await request.Body.CopyToAsync(memStream);
        var bodyContent = memStream.GetString(autoCloseStream:false);
        memStream.Position = 0;
        request.Body = memStream;
        
        var headers = request.Headers.ToJsonString();
        var contentType = request.ContentType;
        var host = request.Host.ToString();
        var url = request.GetDisplayUrl();
        var method = request.Method;
        var path = request.Path;
        var queryString = request.QueryString.ToString();
        var trace = context.TraceIdentifier;
        var connection = context.Connection;
        
        Console.WriteLine(new {bodyContent,headers,contentType,host,url,method, path,queryString,trace, connection = connection.ToString()});
        
        // response
        var originalResponseBody = context.Response.Body;
        using var rspMemoryStream = new MemoryStream();
        context.Response.Body = rspMemoryStream;
        rspMemoryStream.Position = 0;
        
        await _next(context);

        var rspBodyContent = rspMemoryStream.GetString(autoCloseStream:false);
        rspMemoryStream.Position = 0;
        await rspMemoryStream.CopyToAsync(originalResponseBody);
        
        var connection2 = context.Connection;
        var response = context.Response;
        var rspHeader = response.Headers.ToJsonString();
        var statusCode = response.StatusCode.ToString();
        Console.WriteLine(new {connection = connection2.ToString(), trace = context.TraceIdentifier, rspHeader, statusCode,rspBodyContent});
    }

}
