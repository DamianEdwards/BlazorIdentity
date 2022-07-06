using System.Reflection;
using System.Security.Claims;

namespace BlazorIdentity;

public interface IBlazorUserManager<TUser> where TUser : class
{
    bool SupportsUserEmail { get; }
    IdentityOptions Options { get; }
    Task<IdentityResult> CreateAsync(TUser user, string password);
    Task<TUser?> GetUserAsync(ClaimsPrincipal principal);
    Task<string> GetUserIdAsync(TUser user);
    Task<string?> GetUserNameAsync(TUser user);
    Task<string?> GetPhoneNumberAsync(TUser user);
    Task<IdentityResult> SetPhoneNumberAsync(TUser user, string? phoneNumber);
    Task<IdentityResult> ChangePasswordAsync(TUser user, string? currentPassword, string newPassword);
}
