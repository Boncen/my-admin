using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using MyAdmin.Core.Model.Dto;
using MyAdmin.Core.Model.BuildIn;
using MyAdmin.Core.Mvc;
using MyAdmin.Core.Repository;

namespace MyAdmin.ApiHost.Controller;

[ApiController]
[Route("api/[controller]/[action]")]
public class Testv2Controller: CrudController<MaLog, Guid, AddLogDto,LogDto>
{
    public Testv2Controller(IRepository<MaLog,Guid> repository):base(repository)
    {
        
    }
    
}
