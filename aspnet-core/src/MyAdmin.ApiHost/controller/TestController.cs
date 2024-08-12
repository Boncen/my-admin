using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using MyAdmin.Core.Repository;
using ILogger = MyAdmin.Core.Logger.ILogger;

namespace MyAdmin.ApiHost.Controller;

[ApiController]
[ApiVersion("1.0")]
public class TestController : ControllerBase
{
    private readonly ILogger _logger;
    public TestController(ILogger logger)
    {
        _logger = logger;
    }


    [HttpPost("testlog")]
    public Task<string> TestAddLog(string content)
    {
        _logger.LogInformation(content);
        return Task.FromResult("ok");
    }
}
