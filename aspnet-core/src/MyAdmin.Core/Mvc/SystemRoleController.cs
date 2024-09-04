using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyAdmin.Core.Identity;
using MyAdmin.Core.Model.Dto;
using MyAdmin.Core.Model.BuildIn;
using MyAdmin.Core.Repository;

namespace MyAdmin.Core.Mvc;

[ApiController]
[Route("api/[controller]/[action]")]
[Authorize(policy: "admin")]
public class SystemRoleController : CrudController<MaRole, Guid, AddRoleDto, RoleSearchDto, RoleDto>
{
    private readonly ICurrentUser _currentUser;
    private readonly IRepository<MaRole, Guid> _repository;
    public SystemRoleController(IRepository<MaRole, Guid> repository, DBHelper dbHelper, ICurrentUser currentUser) :
        base(repository, dbHelper)
    {
        _currentUser = currentUser;
        _repository = repository;
    }

   // 添加角色
   
   //删除角色
   
   //更新角色
   
   // 列表

   // 分配菜单
}