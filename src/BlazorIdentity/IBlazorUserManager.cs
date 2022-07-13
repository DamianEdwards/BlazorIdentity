using System.Reflection;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
namespace BlazorIdentity;

public interface IBlazorUserManager<TUser> where TUser : class
{
    bool SupportsUserEmail { get; }
    IdentityOptions Options { get; }
    Task<IdentityResult> CreateAsync(TUser user, string password);
    Task<TUser?> GetUserAsync(ClaimsPrincipal principal);
    Task<TUser?> FindByEmailAsync(string email);
    Task<TUser?> FindByIdAsync(string userId);
    Task<string> GetUserIdAsync(TUser user);
    Task<IdentityResult> ConfirmEmailAsync(TUser user, string code);
    Task<IdentityResult> DeleteAsync(TUser user);
	Task<string?> GetUserEmailAsync(TUser user);
    Task<string?> GetAuthenticatorKeyAsync(TUser user);
	Task<bool> HasPasswordAsync(TUser user);
	Task<string> GenerateEmailConfirmationTokenAsync(TUser user);
	Task<string?> GetUserNameAsync(TUser user);
    Task<IList<UserLoginInfo>> GetLoginsAsync(TUser user);
    Task<string?> GetPhoneNumberAsync(TUser user);
    Task<IdentityResult> SetPhoneNumberAsync(TUser user, string? phoneNumber);
    Task<IdentityResult> ChangePasswordAsync(TUser user, string currentPassword, string newPassword);
}
