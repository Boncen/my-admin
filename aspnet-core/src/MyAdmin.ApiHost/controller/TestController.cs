using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using ILogger = MyAdmin.Core.Logger.ILogger;

namespace MyAdmin.ApiHost;

[ApiController]
[ApiVersion("1.0")]
public class TestController : ControllerBase
{
    // private readonly ILogger<TestController> _logger;
    private readonly ILogger _logger;

    public TestController(ILogger logger)
    {
        _logger = logger;
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
        int a = 0;
        int k = 100 / a;
 
        var s =  string.Format("level: {0}", new {level="llle"});

        return Task.FromResult("v1" + s);
    }

    [HttpGet(ApiEndpoints.Test.TestMethod2)]
    [MapToApiVersion("1.0")]
    public Task<string> TestGetv2()
    {
        return Task.FromResult("v1-1");
    }
}
