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
        int a = 0;
        int k = 100 / a;
        return Task.FromResult("v1" + k);
    }

   [HttpGet(ApiEndpoints.Test.TestMethod2)]
   [MapToApiVersion("1.0")]
    public Task<string> TestGetv2(){
        return Task.FromResult("v1-1");
    }
}
