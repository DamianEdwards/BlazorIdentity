using Identity = Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System.Security.Claims;

namespace BlazorIdentity.Server;

internal class BlazorServerUserManager<TUser> : IBlazorUserManager<TUser> where TUser : class
{
    private readonly Identity.UserManager<TUser> _userManager;
    private readonly Identity.IdentityOptions _identityOptions;

    public BlazorServerUserManager(Identity.UserManager<TUser> userManager, IOptionsMonitor<Identity.IdentityOptions> options)
    {
        _userManager = userManager;
        _identityOptions = options.CurrentValue;

        Options = new() { SignIn = new() { RequireConfirmedAccount = _identityOptions.SignIn.RequireConfirmedAccount } };
    }

    public bool SupportsUserEmail => _userManager.SupportsUserEmail;

    public IdentityOptions Options { get; }

    public async Task<IdentityResult> CreateAsync(TUser user, string password)
    {
        var result = await _userManager.CreateAsync(user, password);

        if (result.Succeeded)
        {
            return IdentityResult.Success;
        }

        return IdentityResult.Failed(result.Errors.Select(e => new IdentityError { Code = e.Code, Description = e.Description }).ToArray());
    }

    public Task<TUser?> GetUserAsync(ClaimsPrincipal principal) => _userManager.GetUserAsync(principal);

    public Task<string> GetUserIdAsync(TUser user) => _userManager.GetUserIdAsync(user);

    public Task<string?> GetUserNameAsync(TUser user) => _userManager.GetUserNameAsync(user);

    public Task<string?> GetPhoneNumberAsync(TUser user) => _userManager.GetPhoneNumberAsync(user);

    public async Task<IdentityResult> SetPhoneNumberAsync(TUser user, string? phoneNumber)
    {
        var baseResult = await _userManager.SetPhoneNumberAsync(user, phoneNumber);
        return baseResult.Succeeded switch
        {
            true => IdentityResult.Success,
            _ => IdentityResult.Failed(baseResult.Errors.Select(e => new IdentityError { Code = e.Code, Description = e.Description }))
        };
    }
}
