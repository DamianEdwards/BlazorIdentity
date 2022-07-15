using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System.Security.Claims;

namespace BlazorIdentity.ServerApp.Identity;

public class RevalidatingIdentityAuthenticationStateProvider<TUser>
    : RevalidatingServerAuthenticationStateProvider where TUser : class
{
    private readonly ILogger<RevalidatingIdentityAuthenticationStateProvider<TUser>> _logger;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly Microsoft.AspNetCore.Identity.IdentityOptions _options;
    private readonly SecurityStampValidatorOptions _securityStampValidatorOptions;

    public RevalidatingIdentityAuthenticationStateProvider(
        ILoggerFactory loggerFactory,
        IServiceScopeFactory scopeFactory,
        IOptions<Microsoft.AspNetCore.Identity.IdentityOptions> optionsAccessor,
        IOptions<SecurityStampValidatorOptions> securityStampValidatorOptions)
        : base(loggerFactory)
    {
        _logger = loggerFactory.CreateLogger<RevalidatingIdentityAuthenticationStateProvider<TUser>>();
        _scopeFactory = scopeFactory;
        _options = optionsAccessor.Value;

        _securityStampValidatorOptions = securityStampValidatorOptions.Value;
    }

    // Wire up the Identity security stamp validator interval to the Blazor circuit AuthN state revalidation interval
    protected override TimeSpan RevalidationInterval => _securityStampValidatorOptions.ValidationInterval;

    protected override async Task<bool> ValidateAuthenticationStateAsync(
        AuthenticationState authenticationState, CancellationToken cancellationToken)
    {
        // Get the user manager from a new scope to ensure it fetches fresh data
        var scope = _scopeFactory.CreateScope();
        try
        {
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<TUser>>();
            return await ValidateSecurityStampAsync(userManager, authenticationState.User);
        }
        finally
        {
            if (scope is IAsyncDisposable asyncDisposable)
            {
                await asyncDisposable.DisposeAsync();
            }
            else
            {
                scope.Dispose();
            }
        }
    }

    private async Task<bool> ValidateSecurityStampAsync(UserManager<TUser> userManager, ClaimsPrincipal principal)
    {
        _logger.LogInformation("Validating security stamp for current user");

        var user = await userManager.GetUserAsync(principal);
        if (user == null)
        {
            _logger.LogDebug("User not found, security stamp declared invalid");
            return false;
        }
        else if (!userManager.SupportsUserSecurityStamp)
        {
            _logger.LogDebug("User manager {UserManager} does not support security stamps, security stamp declared valid", userManager.GetType().Name);
            return true;
        }
        else
        {
            var principalStamp = principal.FindFirstValue(_options.ClaimsIdentity.SecurityStampClaimType);
            var userStamp = await userManager.GetSecurityStampAsync(user);
            var isValid = principalStamp == userStamp;

            if (isValid)
            {
                _logger.LogDebug("Security stamp is valid");
                return true;
            }

            _logger.LogDebug("Security stamp is invalid");
            return false;
        }
    }
}
