using System.Security.Claims;

namespace MyAdmin.Core.Identity;

public interface ICurrentUser
{
    LoginedUser? GetCurrentUser(bool throwIfNotFound = false);
}

public record LoginedUser(dynamic? id, string? account, string? role, IEnumerable<Claim> claims);