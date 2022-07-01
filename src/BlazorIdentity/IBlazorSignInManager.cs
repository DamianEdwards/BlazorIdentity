
using System.Security.Claims;

namespace BlazorIdentity;

public interface IBlazorSignInManager<TUser> where TUser : class
{
    Task<SignInResult> PasswordSignInAsync(string userName, string password, bool isPersistent, bool lockoutOnFailure);

    Task SignInAsync(TUser user, bool isPersistent, string? authenticationMethod = null);

    bool IsSignedIn(ClaimsPrincipal user);

    Task SignOutAsync();
}
