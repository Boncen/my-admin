using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyAdmin.Core.Model.Dto;
using MyAdmin.Core.Model.BuildIn;
using MyAdmin.Core.Repository;
using MyAdmin.Core.Model;

namespace MyAdmin.Core.Mvc;

[ApiController]
[Route("api/[controller]/[action]")]
[Authorize(policy: "admin")]
public class SystemMenuController : CrudController<MaMenu, Guid, AddMenuDto, MenuSearchDto, MenuDto>
{
    public SystemMenuController(IRepository<MaMenu, Guid> repository, DBHelper dbHelper) : base(repository, dbHelper)
    {
    }
}