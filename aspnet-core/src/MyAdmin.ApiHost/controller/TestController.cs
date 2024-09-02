using Microsoft.AspNetCore.Mvc;
using MyAdmin.Core.Model.BuildIn;
using MyAdmin.Core.Repository;

namespace MyAdmin.ApiHost.Controller;

[ApiController]
[Route("/api/[controller]/[action]")]
public class TestController : ControllerBase
{
    private readonly DBHelper _dbHelper;
    private readonly IRepository<MaUser, Guid> _repository;

    public TestController(DBHelper dbHelper, IRepository<MaUser, Guid> repository)
    {
        _dbHelper = dbHelper;
        _repository = repository;
    }
}