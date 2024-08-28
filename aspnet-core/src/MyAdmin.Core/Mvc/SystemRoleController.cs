using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using MyAdmin.Core.Model.Dto;
using MyAdmin.Core.Model.BuildIn;
using MyAdmin.Core.Mvc;
using MyAdmin.Core.Repository;

namespace MyAdmin.Core.Mvc;

[ApiController]
[Route("api/[controller]/[action]")]
public class SystemRoleController: CrudController<MaRole, Guid, AddRoleDto,RoleDto>
{
    public SystemRoleController(IRepository<MaRole,Guid> repository, DBHelper dbHelper):base(repository,dbHelper)
    {
        
    }

    [HttpGet]
    public string TestThrowException(string content)
    {
        throw new System.Exception(content);
    }
}
