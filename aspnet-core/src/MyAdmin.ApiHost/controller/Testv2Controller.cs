using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using MyAdmin.ApiHost.Dto;
using MyAdmin.Core.Model.BuildIn;
using MyAdmin.Core.Mvc;
using MyAdmin.Core.Repository;

namespace MyAdmin.ApiHost.Controller;

[ApiController]
[Route("api/[controller]/[action]")]
public class Testv2Controller: CrudController<Log, Guid, AddLogDto,LogDto>
{
    public Testv2Controller(IRepository<Log,Guid> repository):base(repository)
    {
        
    }
    
}
