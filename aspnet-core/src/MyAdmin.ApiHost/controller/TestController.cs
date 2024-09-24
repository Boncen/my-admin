using Microsoft.AspNetCore.Mvc;
using MyAdmin.ApiHost.Db;
using MyAdmin.Core.Model.BuildIn;
using MyAdmin.Core.Repository;
using Order = MyAdmin.ApiHost.models.Order;

namespace MyAdmin.ApiHost.Controller;

[ApiController]
[Route("/api/[controller]/[action]")]
public class TestController : ControllerBase
{
    private readonly DBHelper _dbHelper;
    private readonly IRepository<MaUser, Guid> _repository;
    private readonly IRepository<Order, Guid, AdminTemplateDbContext> _orderRepository;

    public TestController(DBHelper dbHelper, IRepository<MaUser, Guid> repository, IRepository<Order, Guid,AdminTemplateDbContext> orderRepository)
    {
        _dbHelper = dbHelper;
        _repository = repository;
        _orderRepository = orderRepository;
    }

    [HttpGet]
    public Task<MaUser> GetUser()
    {
        var user = _repository.FindOneAsync(x => x.Name.Length < 6);
        return user;
    }
    
    [HttpGet]
    public Task<Order> GetOrder()
    {
        var order = _orderRepository.FindOneAsync(x => x.OrderNo == "123456");
        return order;
    }
}