using Asp.Versioning;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using MyAdmin.ApiHost.Service;
using MyAdmin.Core.Model.BuildIn;
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
    [HttpGet("test1")]
    public Task<string> Test1(string content, int a)
    {
        return Task.FromResult(content + a);
    }
    [HttpPost("test2")]
    public async Task<object> Test2([FromBody]Param1 obj)
    {
        return obj;
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
    
    [HttpPost("dapper2")]
    public async Task<int> TestDapper2([FromServices]DBHelper helper, Guid id)
    {
        var count = await helper.InsertAsync<Log>(new Log(){Id = Guid.NewGuid(), Content = "test", LogTime = DateTime.Now});
        return count;
    }
}

public class Param1
{
    public string P1 { get; set; }
    public string P2 { get; set; }
}