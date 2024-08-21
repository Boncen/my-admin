﻿using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MyAdmin.Core.Model.BuildIn;
using MyAdmin.Core.Options;
using MyAdmin.Core.Repository;
using MyAdmin.Core.Utilities;
using ILogger = MyAdmin.Core.Logger.ILogger;

namespace MyAdmin.Core.Framework.Middlewares;

public class RequestMonitorMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger _logger;
    // private readonly DBHelper _dbHelper;
    private readonly MaFrameworkOptions _maFrameworkOptions;

    public RequestMonitorMiddleware(RequestDelegate next, IOptions<MaFrameworkOptions> options,ILogger logger)
    {
        _next = next;
        _logger = logger;
        // _dbHelper = dbHelper;
        _maFrameworkOptions = options.Value;
    }

    public async Task Invoke(HttpContext context)
    {
        var request = context.Request;
        var ip = context.Connection.RemoteIpAddress?.ToString();
        var bodyContent = string.Empty;
        var memStream = new MemoryStream();
        if (_maFrameworkOptions.SaveRequestBody == true)
        {
            await request.Body.CopyToAsync(memStream);
            bodyContent = memStream.GetStringByPipe();
            memStream.Position = 0;
            request.Body = memStream;
        }
        
        var contentType = request.ContentType;
        var host = request.Host.ToString();
        var url = request.GetDisplayUrl();
        var method = request.Method;
        
        await _next(context);

        var rspBodyContent = string.Empty;
        var rspMemoryStream = new MemoryStream();
        if (_maFrameworkOptions.SaveResponseBody == true)
        {
            var originalResponseBody = context.Response.Body;
            context.Response.Body = rspMemoryStream;
            rspMemoryStream.Position = 0;
            rspBodyContent = rspMemoryStream.GetStringByPipe();
            rspMemoryStream.Position = 0;
            await rspMemoryStream.CopyToAsync(originalResponseBody);
        }
        
        var response = context.Response;
        var statusCode = response.StatusCode.ToString();
        // var dbHelper = context.RequestServices.GetService(typeof(DBHelper)) as DBHelper;
        // dbHelper.Connection.Execute($"insert into {nameof(MaLog)} (Id,ContentType,Host,HttpMethod,IpAddress,Level,LogTime,Origin,Referer,Url,RequestBody,ResponseStatusCode,ResponseBody,UserAgent) values (@Id, @ContentType,@Host,@HttpMethod,@IpAddress,@Level,@LogTime,@Origin,@Referer,@Url,@RequestBody,@ResponseStatusCode,@ResponseBody,@UserAgent)",
        //     new MaLog()
        //     {
        //         Id = Guid.NewGuid(),
        //         Content = "",
        //         ContentType = contentType,
        //         Exceptions = String.Empty,
        //         Host = host,
        //         HttpMethod = method,
        //         IpAddress = ip,
        //         Level = LogLevel.Trace,
        //         LogTime = DateTime.Now,
        //         Origin = request.Headers["Origin"],
        //         Referer = request.Headers["Referer"],
        //         Url = url,
        //         RequestBody = bodyContent,
        //         ResponseStatusCode = statusCode,
        //         ResponseBody = rspBodyContent,
        //         UserAgent = request.Headers["User-Agent"]
        //     }
        //     );
        
        _logger.Log(new MaLog()
        {
            Id = Guid.NewGuid(),
            Content = "",
            ContentType = contentType,
            Exceptions = String.Empty,
            Host = host,
            HttpMethod = method,
            IpAddress = ip,
            Level = LogLevel.Trace,
            LogTime = DateTime.Now,
            Origin = request.Headers["Origin"],
            Referer = request.Headers["Referer"],
            Url = url,
            RequestBody = bodyContent,
            ResponseStatusCode = statusCode,
            ResponseBody = rspBodyContent,
            UserAgent = request.Headers["User-Agent"]
        });

        if (memStream != null)
        {
            memStream.Close();
            memStream.Dispose();
        }
        if (rspMemoryStream != null)
        {
            rspMemoryStream.Close();
            rspMemoryStream.Dispose();
        }
    }

}
