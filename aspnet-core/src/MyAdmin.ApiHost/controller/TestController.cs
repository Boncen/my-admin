using System.ComponentModel.DataAnnotations;
using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using MyAdmin.ApiHost.Db;
using MyAdmin.ApiHost.models;
using MyAdmin.ApiHost.Service;
using MyAdmin.Core.Framework.Attribute;
using MyAdmin.Core.Mvc;
using MyAdmin.Core.Repository;
using ILogger = MyAdmin.Core.Logger.ILogger;

namespace MyAdmin.ApiHost.Controller;


[ApiVersion("1.0")]
public class TestController : MAController
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
        Console.WriteLine("Test2 Test2");
        return obj;
    }
    [HttpPost("test3")]
    [IgnoreRequestLog]
    public async Task<object> Test3([FromBody]Param1 obj)
    {
        Console.WriteLine("Test3");
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
    [HttpPost("testattrinjectservice")]
    public Task<string> TestService3([FromServices]TestServiceAttr serv)
    {
        return Task.FromResult(serv.GetServiceName());
    }
    [HttpPost("testattrinjectkeyedservice")]
    public Task<string> TestService4([FromKeyedServices("HelloKeyedTest")]TestKeyedServiceAttr serv)
    {
        return Task.FromResult(serv.GetServiceName());
    }
    [HttpPost("order")]
    public async Task<Order> TestDapper2([FromServices]IRepository<Order,Guid,AdminTemplateDbContext> orderRep, Guid id)
    {
        var count = await orderRep.InsertAsync(new Order()
        {
            Id = Guid.NewGuid(),
            Amount = 199,
            DescBody = "炸酱面",
            OrderNo = "1"
        },autoSave:true);
        return count;
    }
}

public class Param1
{
    [Required(ErrorMessage = "need P1")]
    public string P1 { get; set; }
    public string P2 { get; set; }
}