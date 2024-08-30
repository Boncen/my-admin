using System.Security.Claims;

namespace MyAdmin.Core.Identity;

public interface ICurrentUser
{
    LoginedUser GetCurrentUser();
}

public record LoginedUser(dynamic id, string account, string role, IEnumerable<Claim> claims);