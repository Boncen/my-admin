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
public class TenantController : CrudController<MaTenant, Guid, AddTenantDto, TenantSearchDto, TenantDto>
{
    public TenantController(IRepository<MaTenant, Guid> repository, DBHelper dbHelper) : base(repository, dbHelper)
    {
    }
}