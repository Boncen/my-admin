using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;

namespace MyAdmin.ApiHost;

[ApiController]
[ApiVersion("2.0")]
public class Testv2Controller: ControllerBase
{
    [HttpGet(ApiEndpoints.Testv2.TestMethod)]
    [MapToApiVersion("2.0")]
    public Task<string> TestGet(){
        return Task.FromResult("v2");
    }

    [HttpGet(ApiEndpoints.Testv2.TestMethod2)]
    [MapToApiVersion("2.0")]
    public Task<string> TestGetv2(){
        return Task.FromResult("v2");
    }
}
