using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
using System.Security.Claims;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyAdmin.Core.Identity;
using MyAdmin.Core.Model;
using MyAdmin.Core.Model.BuildIn;
using MyAdmin.Core.Model.Dto;
using MyAdmin.Core.Mvc;
using MyAdmin.Core.Repository;
using MyAdmin.Core.Utilities;

namespace MyAdmin.ApiHost.Controller;

[Route("api/[controller]/[action]")]
public class UserController : MAController
{
    // private readonly ILogger _logger;
    private readonly IRepository<MaUser, Guid> _repository;
    private readonly IRepository<MaRole, Guid> _roleRepository;
    private readonly JwtHelper _jwtHelper;
    private readonly ICurrentUser _currentUser;

    public UserController(IRepository<MaUser, Guid> repository, JwtHelper jwtHelper, IRepository<MaRole, Guid> roleRepository, ICurrentUser currentUser)
    {
        // _logger = logger;
        _repository = repository;
        _jwtHelper = jwtHelper;
        _roleRepository = roleRepository;
        _currentUser = currentUser;
    }

    // todo 生成验证码

    /// <summary>
    /// 登录
    /// </summary>
    /// <param name="req"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPost]
    [AllowAnonymous]
    public async Task<ApiResult> Login([FromBody] LoginReq req,
        [FromServices] IRepository<MaTenant, Guid> _tenantRepository,
        [FromServices] IRepository<UserRole> _userRoleRepository,
         CancellationToken cancellationToken)
    {
        MaTenant? tenant = null;
        if (req.tenantId.HasValue)
        {
            tenant = await _tenantRepository.GetByIdAsync(req.tenantId.Value, cancellationToken);
            if (tenant == null)
            {
                return ApiResult.Fail("租户不存在");
            }

            if (tenant.ExpirationDate.HasValue && DateTime.Now > tenant.ExpirationDate.Value)
            {
                return ApiResult.Fail("租户已过有效期");
            }
        }

        var user = await _repository.FindOneAsync(
            x => x.Account == req.account && x.TenantId == req.tenantId && x.IsEnabled == true && x.IsDeleted == false,
            cancellationToken);
        if (user == null)
        {
            return ApiResult.Fail("登录名或者密码错误");
        }

        var hashPasswd = PasswordHelper.HashPassword(req.password, user.Salt!);
        if (!user.Password.Equals(hashPasswd))
        {
            return ApiResult.Fail("登录名或者密码错误!");
        }

        // generate JWT token
        var claims = new List<Claim>
        {
            new Claim("account", user.Account),
            new Claim("id", user.Id.ToString()),
        };

        var userRoles = await _userRoleRepository.GetListAsync(x => x.UserId == user.Id);
        if (userRoles.Count > 0)
        {
            var roleIds = userRoles.Select(x => x.RoleId);
            var roles = await _roleRepository.GetListAsync(r => roleIds.Contains(r.Id));
            var roleNames = roles.Select(x => x.Name).ToList();
            claims.Add(new Claim(ClaimTypes.Role, string.Join(',', roleNames)));
        }
        var token = _jwtHelper.CreateToken(claims);
        return ApiResult<string>.Ok(token);
    }

    /// <summary>
    /// 登出
    /// </summary>
    /// <returns></returns>
    [HttpPost]
    public ApiResult Logout([FromServices] TokenBlackList _tokenBlackList)
    {
        var token = _jwtHelper.GetUserToken();
        _tokenBlackList.AddToken(token);
        return ApiResult.Ok();
    }

    /// <summary>
    /// 获取菜单
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    public async Task<ApiResult<List<MenuDto>>> Menus(
        [FromServices] IRepository<RoleMenu> roleMenuRepository,
         [FromServices] IRepository<UserRole> userRoleRepository,
        [FromServices] IRepository<MaMenu> menuRepo,
        [FromQuery] bool excludeButton = true
        )
    {
        userRoleRepository.IsChangeTrackingEnabled = false;
        roleMenuRepository.IsChangeTrackingEnabled = false;
        menuRepo.IsChangeTrackingEnabled = false;
        // get user role
        var user = _currentUser.GetCurrentUser(true);
        var userId = Guid.Parse(user!.id!);

        List<MaMenu> menus = new List<MaMenu>();
        var menuSet = menuRepo.GetDbSet();
        var menuWhere = menuSet.Where(x => x.IsDeleted == false);
        if (excludeButton)
        {
            menuWhere = menuWhere.Where(x => (x.MenuType == MenuType.Page || x.MenuType == MenuType.Category) );
        }
        
        if (user!.IsDev())
        {
            // 开发者角色获取全部
            menus = await menuWhere.AsNoTracking().ToListAsync();
        }
        else
        {
            // 获取用户所有角色id
            var roleIds = (await userRoleRepository.GetListAsync(x => x.UserId == userId)).Select(x => x.RoleId);

            // 获取角色菜单
            var menusId = (await roleMenuRepository.GetListAsync(x => roleIds.Contains(x.RoleId))).Select(x => x.MenuId);
            if (menusId.Count() > 0)
            {
                menuWhere = menuWhere.Where(x=>menusId.Contains(x.Id));
                menus = await menuWhere.AsNoTracking().ToListAsync();
            }
        }
        List<MenuDto> menuDtos = menus.Adapt<List<MenuDto>>();
        for (int i = 0; i < menuDtos.Count(); i++)
        {
            var m = menuDtos[i];
            if (m.ParentId != null)
            {
                var parent = menuDtos.FirstOrDefault(x => x.Id == m.ParentId);
                if (parent == null)
                {
                    continue;
                }
                if (parent.Children == null)
                {
                    parent.Children = new List<MenuDto>();
                }
                parent.Children.Add(m);
            }
        }

        return ApiResult<List<MenuDto>>.Ok(menuDtos.Where(x => x.ParentId == null).ToList());
    }

}

public record LoginReq([Required] string account, [Required] string password, Guid? tenantId);