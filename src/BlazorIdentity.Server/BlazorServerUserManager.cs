using Identity = Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

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

    public Task<string> GetUserIdAsync(TUser user)
    {
        return _userManager.GetUserIdAsync(user);
    }
}
