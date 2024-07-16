using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;

namespace MyAdmin.ApiHost;

[ApiController]
[ApiVersion("1.0")]
public class TestController: ControllerBase
{
    [HttpGet(ApiEndpoints.Test.TestMethod)]
    [MapToApiVersion("1.0")]
    public Task<string> TestGet(){
        return Task.FromResult("v1");
    }

   [HttpGet(ApiEndpoints.Test.TestMethod2)]
   [MapToApiVersion("1.0")]
    public Task<string> TestGetv2(){
        return Task.FromResult("v1-1");
    }
}
