using System.Security.Claims;
using Identity = Microsoft.AspNetCore.Identity;

namespace BlazorIdentity.Server;

internal class BlazorServerUserManager<TUser> : IBlazorUserManager<TUser> where TUser : class
{
    private readonly Identity.UserManager<TUser> _userManager;
    private readonly Identity.IdentityOptions _identityOptions;

    public BlazorServerUserManager(Identity.UserManager<TUser> userManager)
    {
        _userManager = userManager;
        _identityOptions = userManager.Options;

        Options = new() { SignIn = new() { RequireConfirmedAccount = _identityOptions.SignIn.RequireConfirmedAccount } };
    }

    public bool SupportsUserEmail => _userManager.SupportsUserEmail;

    public IdentityOptions Options { get; }

    public async Task<IdentityResult> CreateAsync(TUser user, string password)
        => ToBlazorIdentityResult(await _userManager.CreateAsync(user, password));

    public Task<TUser?> GetUserAsync(ClaimsPrincipal principal) => _userManager.GetUserAsync(principal);

    public Task<string> GetUserIdAsync(TUser user) => _userManager.GetUserIdAsync(user);

    public Task<string?> GetUserNameAsync(TUser user) => _userManager.GetUserNameAsync(user);

    public Task<string?> GetPhoneNumberAsync(TUser user) => _userManager.GetPhoneNumberAsync(user);

    public async Task<IdentityResult> SetPhoneNumberAsync(TUser user, string? phoneNumber)
        => ToBlazorIdentityResult(await _userManager.SetPhoneNumberAsync(user, phoneNumber));

    public async Task<IdentityResult> ChangePasswordAsync(TUser user, string? currentPassword, string newPassword)
        => ToBlazorIdentityResult(await _userManager.ChangePasswordAsync(user, currentPassword!, newPassword));

    private static IdentityResult ToBlazorIdentityResult(Identity.IdentityResult result)
    {
        return result.Succeeded switch
        {
            true => IdentityResult.Success,
            _ => IdentityResult.Failed(result.Errors.Select(e => new IdentityError { Code = e.Code, Description = e.Description }))
        };
    }
}
