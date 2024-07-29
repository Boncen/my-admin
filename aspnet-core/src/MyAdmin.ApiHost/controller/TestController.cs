﻿using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using MyAdmin.ApiHost.db;
using MyAdmin.Core.Repository;
using ILogger = MyAdmin.Core.Logger.ILogger;

namespace MyAdmin.ApiHost;

[ApiController]
[ApiVersion("1.0")]
public class TestController : ControllerBase
{
    // private readonly ILogger<TestController> _logger;
    private readonly ILogger _logger;
    private readonly IRepository<Log,Guid> _logRepository;
    private readonly ILogRepository _logRe;
    public TestController(ILogger logger, IRepository<Log,Guid> logRepository,ILogRepository logRe)
    {
        _logger = logger;
        _logRepository = logRepository;
        _logRe = logRe;
    }


    [HttpGet(ApiEndpoints.Test.TestMethod)]
    [MapToApiVersion("1.0")]
    public Task<string> TestGet()
    {
        _logger.LogDebug("测试日志打印到控制台的样子");
        _logger.LogCritical("测试日志打印到控制台的样子");
        _logger.LogInformation("测试日志打印到控制台的样子");
        _logger.LogTrace("测试日志打印到控制台的样子");
        _logger.LogWarning("测试日志打印到控制台的样子");
        
        _logRe.InsertAsync(new Log()
        {
            Id = Guid.NewGuid(),
            Content = "Test2"
        }, true);
        // int a = 0;
        // int k = 100 / a;
 
        var s =  string.Format("level: {0}", new {level="llle"});

        return Task.FromResult("v1" + s);
    }

    [HttpGet(ApiEndpoints.Test.TestMethod2)]
    [MapToApiVersion("1.0")]
    public Task<string> TestGetv2(CancellationToken cancellationToken)
    {
        _logRepository.InsertAsync(new Log()
        {
            Id = Guid.NewGuid(),
            Content = "Test"
        }, true, cancellationToken);
        return Task.FromResult("v1-1");
    }
}
