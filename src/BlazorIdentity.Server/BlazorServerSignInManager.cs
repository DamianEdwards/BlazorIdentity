using System.Linq;

namespace BlazorIdentity.Server;

using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Http;
using Identity = Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.JSInterop;

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

        return new()
        {
            Succeeded = result.Succeeded,
            IsLockedOut = result.IsLockedOut,
            IsNotAllowed = result.IsNotAllowed,
            RequiresTwoFactor = result.RequiresTwoFactor
        };
    }

    async Task<SignInResult> IBlazorSignInManager<TUser>.TwoFactorRecoveryCodeSignInAsync(string recoveryCode)
    {
        var result = await base.TwoFactorRecoveryCodeSignInAsync(recoveryCode);

        return new()
        {
            Succeeded = result.Succeeded,
            IsLockedOut = result.IsLockedOut,
            IsNotAllowed = result.IsNotAllowed,
            RequiresTwoFactor = result.RequiresTwoFactor
        };
    }

    async Task<SignInResult> IBlazorSignInManager<TUser>.TwoFactorAuthenticatorSignInAsync(string authenticatorCode, bool rememberMe, bool rmemberMachine)
    {
        var result = await base.TwoFactorAuthenticatorSignInAsync(authenticatorCode, rememberMe, rmemberMachine);

        return new()
        {
            Succeeded = result.Succeeded,
            IsLockedOut = result.IsLockedOut,
            IsNotAllowed = result.IsNotAllowed,
            RequiresTwoFactor = result.RequiresTwoFactor
        };
    }

    async Task<TUser> IBlazorSignInManager<TUser>.GetTwoFactorAuthenticationUserAsync()
    {
        var result = await base.GetTwoFactorAuthenticationUserAsync();

        return result!;
    }

    Task IBlazorSignInManager<TUser>.SignInAsync(TUser user, bool isPersistent, string? authenticationMethod)
    {
        return base.SignInAsync(user, isPersistent, authenticationMethod);
    }

    Task IBlazorSignInManager<TUser>.RefreshSignInAsync(TUser user)
    {
        return base.RefreshSignInAsync(user);
    }

    async Task<List<BlazorIdentity.AuthenticationScheme>> IBlazorSignInManager<TUser>.GetExternalAuthenticationSchemesAsync()
    {
        var schemes = await base.GetExternalAuthenticationSchemesAsync();

        var returnValue = new List<BlazorIdentity.AuthenticationScheme>();

        foreach (var scheme in schemes)
        {
            // TODO: Not sure why I need to do this right now. Something to do with how Blazor Identity cookie scheme is defined
            if (!scheme.Name.Equals("Identity.Application", StringComparison.CurrentCultureIgnoreCase))
            {
                var e = new BlazorIdentity.AuthenticationScheme
                {
                    Name = scheme.Name,
                    DisplayName = scheme.DisplayName ?? string.Empty,
                    HandlerType = scheme.HandlerType
                };
                returnValue.Add(e);
            }
        }
        return returnValue;
    }

    async Task<ExternalLoginInfo?> IBlazorSignInManager<TUser>.GetExternalLoginInfoAsync()
    {
        var result = await base.GetExternalLoginInfoAsync();

        var externalLoginInfo = new ExternalLoginInfo(result!.Principal, result.LoginProvider, result.ProviderKey, result.ProviderDisplayName!);

        var authTokens = (from token in result.AuthenticationTokens!
                          let t = new AuthToken
                          {
                              Name = token.Name,
                              Value = token.Value
                          }
                          select t).ToList();

        externalLoginInfo.AuthenticationTokens = authTokens;

        return externalLoginInfo;
    }

    async Task<SignInResult> IBlazorSignInManager<TUser>.ExternalLoginSignInInAsync(string loginProvider, string providerKey, bool isPersistent, bool bypassTwoFactor)
    {
        var result = await base.ExternalLoginSignInAsync(loginProvider, providerKey, isPersistent, bypassTwoFactor);

        return ConvertSignInResult(result);
    }

    private static SignInResult ConvertSignInResult(Identity.SignInResult result)
    {
        return new SignInResult
        {
            Succeeded = result.Succeeded,
            IsLockedOut = result.IsLockedOut,
            IsNotAllowed = result.IsNotAllowed,
            RequiresTwoFactor = result.RequiresTwoFactor,
        };
    }

    async Task<ClaimsPrincipal> IBlazorSignInManager<TUser>.CreateUserPrincipalAsync(TUser user)
    {
        return await base.CreateUserPrincipalAsync(user);
    }
}
