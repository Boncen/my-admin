using System.Diagnostics;
using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyAdmin.Core.Identity;
using MyAdmin.Core.Model.Dto;
using MyAdmin.Core.Model.BuildIn;
using MyAdmin.Core.Mvc;
using MyAdmin.Core.Repository;

namespace MyAdmin.Core.Mvc;

[ApiController]
[Route("api/[controller]/[action]")]
[Authorize(policy: "admin")]
public class SystemRoleController : CrudController<MaRole, Guid, AddRoleDto, RoleSearchDto, RoleDto>
{
    private readonly ICurrentUser _currentUser;

    public SystemRoleController(IRepository<MaRole, Guid> repository, DBHelper dbHelper, ICurrentUser currentUser) :
        base(repository, dbHelper)
    {
        _currentUser = currentUser;
    }

    [HttpGet]
    public string TestThrowException(string content)
    {
        throw new System.Exception(content);
    }
}