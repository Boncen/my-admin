using Microsoft.AspNetCore.Mvc;

namespace MyAdmin.ApiHost;

[ApiController]
[Route("api/[controller]/[action]")]
public class TestController: ControllerBase
{

    [HttpGet]
    public void TestGet(){

    }
}
