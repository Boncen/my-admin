using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using MyAdmin.ApiHost.Service;
using MyAdmin.Core.Repository;
using ILogger = MyAdmin.Core.Logger.ILogger;

namespace MyAdmin.ApiHost.Controller;

[ApiController]
[ApiVersion("1.0")]
public class TestController : ControllerBase
{
    private readonly ILogger _logger;
    private readonly TestService _testService;
    public TestController(ILogger logger, TestService testService)
    {
        _logger = logger;
        _testService = testService;
    }


    [HttpPost("testlog")]
    public Task<string> TestAddLog(string content)
    {
        _logger.LogInformation(content);
        return Task.FromResult("ok");
    }
    
    [HttpPost("testserv")]
    public Task<string> TestService()
    {
        return Task.FromResult(_testService.GetServiceName());
    }
    [HttpPost("testsington")]
    public Task<string> TestService2([FromServices]TestSingtonService serv)
    {
        return Task.FromResult(serv.GetServiceName());
    }
}
