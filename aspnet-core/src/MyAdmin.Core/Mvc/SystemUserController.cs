using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyAdmin.Core.Model.Dto;
using MyAdmin.Core.Model.BuildIn;
using MyAdmin.Core.Mvc;
using MyAdmin.Core.Repository;

namespace MyAdmin.Core.Mvc;

[ApiController]
[Route("api/[controller]/[action]")]
[Authorize(policy: "admin")]
public class SystemUserController : CrudController<MaUser, Guid, AddUserDto, UserSearchDto, UserDto>
{
    public SystemUserController(IRepository<MaUser, Guid> repository, DBHelper dbHelper) : base(repository, dbHelper)
    {
    }
}