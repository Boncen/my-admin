using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using MyAdmin.ApiHost.Dto;
using MyAdmin.Core.Mvc;
using MyAdmin.Core.Repository;

namespace MyAdmin.ApiHost;

[ApiController]
[ApiVersion("2.0")]
[Route("api/[controller]/[action]")]
public class Testv2Controller: CrudController<Log, Guid, AddLogDto,LogDto>
{
    public Testv2Controller(IRepository<Log,Guid> repository):base(repository, nameof(Log))
    {
        
    }
    
}
