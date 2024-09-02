using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyAdmin.Core.Model;
using MyAdmin.Core.Model.BuildIn;
using MyAdmin.Core.Mvc;
using MyAdmin.Core.Repository;
using MyAdmin.Core.Utilities;

namespace MyAdmin.ApiHost.Controller;

[Route("/login")]
[AllowAnonymous]
public class LoginController : MAController
{
    // private readonly ILogger _logger;
    private readonly IRepository<MaUser, Guid> _repository;
    private readonly IRepository<MaTenant, Guid> _tenantRepository;
    private readonly IRepository<MaRole, Guid> _roleRepository;
    private readonly JwtHelper _jwtHelper;

    public LoginController(IRepository<MaUser, Guid> repository, IRepository<MaTenant, Guid> tenantRepository,
        JwtHelper jwtHelper, IRepository<MaRole, Guid> roleRepository)
    {
        // _logger = logger;
        _repository = repository;
        _tenantRepository = tenantRepository;
        _jwtHelper = jwtHelper;
        _roleRepository = roleRepository;
    }

    // todo 生成验证码

    /// <summary>
    /// 登录
    /// </summary>
    /// <param name="req"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<ApiResult> Login([FromBody] LoginReq req, CancellationToken cancellationToken)
    {
        MaTenant tenant = null;
        if (req.tenantId.HasValue)
        {
            tenant = await  _tenantRepository.GetByIdAsync(req.tenantId.Value, cancellationToken);
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
            cancellationToken, maUser => maUser.UserRoles);
        if (user == null)
        {
            return ApiResult.Fail("登录名或者密码错误");
        }

        var hashPasswd = PasswordHelper.HashPassword(req.password, user.Salt);
        if (!user.Password.Equals(hashPasswd))
        {
            return ApiResult.Fail("登录名或者密码错误!");
        }

        // get user's roles
        var roles = await _roleRepository.GetListAsync(r => user.UserRoles.Select(x => x.RoleId).Contains(r.Id));
        var roleNames = roles.Select(x => x.Name).ToList();
        // generate JWT token
        var claims = new[]
        {
            new Claim(ClaimTypes.Role, string.Join(',', roleNames)),
            new Claim("account", user.Account),
            new Claim("id", user.Id.ToString()),
        };
        
        var token = _jwtHelper.CreateToken(claims);
        return ApiResult<string>.Ok(token);
    }
}

public record LoginReq([Required] string account, [Required] string password, Guid? tenantId);