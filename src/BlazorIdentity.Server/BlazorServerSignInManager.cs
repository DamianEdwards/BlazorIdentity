using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Http;
using Identity = Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.JSInterop;

namespace BlazorIdentity.Server;

internal class BlazorServerSignInManager<TUser> : Identity.SignInManager<TUser>, IBlazorSignInManager<TUser> where TUser : class
{
    public BlazorServerSignInManager(Identity.UserManager<TUser> userManager,
        IHttpContextAccessor contextAccessor,
        AuthenticationStateProvider authenticationStateProvider,
        IJSRuntime jsRuntime,
        Identity.IUserClaimsPrincipalFactory<TUser> claimsFactory,
        IOptions<Identity.IdentityOptions> optionsAccessor,
        ILogger<BlazorServerSignInManager<TUser>> logger,
        IAuthenticationSchemeProvider schemes,
        Identity.IUserConfirmation<TUser> confirmation)
        : base(userManager, contextAccessor, claimsFactory, optionsAccessor, logger, schemes, confirmation)
    {
        ArgumentNullException.ThrowIfNull(userManager);
        ArgumentNullException.ThrowIfNull(contextAccessor);
        ArgumentNullException.ThrowIfNull(claimsFactory);

        AuthenticationStateProvider = authenticationStateProvider;
        JSRuntime = jsRuntime;
    }

    public virtual AuthenticationStateProvider AuthenticationStateProvider { get; }
    
    public virtual IJSRuntime JSRuntime { get; private set; }

    public override async Task SignInWithClaimsAsync(TUser user, AuthenticationProperties? authenticationProperties, IEnumerable<Claim> additionalClaims)
    {
        var userPrincipal = await CreateUserPrincipalAsync(user);
        foreach (var claim in additionalClaims)
        {
            userPrincipal.Identities.First().AddClaim(claim);
        }

        var authProperties = authenticationProperties ?? new AuthenticationProperties();
        authProperties.Parameters[nameof(IJSRuntime)] = JSRuntime;
        authProperties.Parameters[nameof(IHostEnvironmentAuthenticationStateProvider)] = AuthenticationStateProvider;

        await Context.SignInAsync(Identity.IdentityConstants.ApplicationScheme,
            userPrincipal,
            authProperties);
    }

    public override async Task SignOutAsync()
    {
        var authProperties = new AuthenticationProperties();
        authProperties.Parameters[nameof(IJSRuntime)] = JSRuntime;
        authProperties.Parameters[nameof(IHostEnvironmentAuthenticationStateProvider)] = AuthenticationStateProvider;

        await Context.SignOutAsync(Identity.IdentityConstants.ApplicationScheme, authProperties);
        //await Context.SignOutAsync(Identity.IdentityConstants.ExternalScheme, authProperties);
        //await Context.SignOutAsync(Identity.IdentityConstants.TwoFactorUserIdScheme, authProperties);
    }

    async Task<SignInResult> IBlazorSignInManager<TUser>.PasswordSignInAsync(string userName, string password, bool isPersistent, bool lockoutOnFailure)
    {
        var result = await base.PasswordSignInAsync(userName, password, isPersistent, lockoutOnFailure);

        return new() { Succeeded = result.Succeeded };
    }

    Task IBlazorSignInManager<TUser>.SignInAsync(TUser user, bool isPersistent, string? authenticationMethod)
    {
        return base.SignInAsync(user, isPersistent, authenticationMethod);
    }
}
