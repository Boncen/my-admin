using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace MyAdmin.Core.Identity;

public class CurrentUser:ICurrentUser
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    public CurrentUser(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }
    public LoginedUser? GetCurrentUser()
    {
        var user = _httpContextAccessor.HttpContext?.User;
        if (user == null || user.Claims.Count() < 1)
        {
            return null;
        }
        var claims = user.Claims;
        var id = GetClaimByType(nameof(LoginedUser.id), claims)?.Value;
        var account = GetClaimByType(nameof(LoginedUser.account), claims)?.Value;
        var role = GetClaimByType(ClaimTypes.Role, claims)?.Value;
        return new LoginedUser(id,account,role,user.Claims);
    }

    private Claim? GetClaimByType(string typeName, IEnumerable<Claim> claims)
    {
        foreach (var claim in claims)
        {
            if (claim.Type.ToString().Equals(typeName))
            {
                return claim;
            }
        }
        return null;
    }
}