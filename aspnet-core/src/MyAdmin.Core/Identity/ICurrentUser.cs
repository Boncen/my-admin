using System.Security.Claims;

namespace MyAdmin.Core.Identity;

public interface ICurrentUser
{
    LoginedUser? GetCurrentUser(bool throwIfNotFound = false);
}

public record LoginedUser(string? id, string? account, string? role, IEnumerable<Claim> claims);


public static class CurrentUserExt
{
    public static bool IsDev(this LoginedUser loginedUser)
    {
        if (loginedUser == null)
        {
            return false;
        }
        return loginedUser.role?.Contains("dev") ?? false;
    }
}