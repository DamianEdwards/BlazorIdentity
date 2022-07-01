using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.JSInterop;

namespace BlazorIdentity.Server;

internal class BlazorServerAwareCookieAuthenticationHandler : CookieAuthenticationHandler
{
    public BlazorServerAwareCookieAuthenticationHandler(
        IOptionsMonitor<CookieAuthenticationOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        ISystemClock clock)
        : base(options, logger, encoder, clock)
    {

    }

    protected override async Task HandleSignInAsync(ClaimsPrincipal user, AuthenticationProperties? properties)
    {
        ArgumentNullException.ThrowIfNull(user);

        if (IsComponentHubConnection())
        {
            ExtractBlazorConstructs(properties, out var authStateProvider, out var jsRuntime);

            authStateProvider.SetAuthenticationState(Task.FromResult(new AuthenticationState(user)));
            var jsModuleTask = jsRuntime.InvokeAsync<IJSObjectReference>("import", "./_content/BlazorIdentity.Server/BlazorIdentity.js");

            // Initiate request from client to endpoint to retrieve auth cookie
            var ticket = new AuthenticationTicket(user, Scheme.Name);
            var ticketValue = Options.TicketDataFormat.Protect(ticket);
            var jsModule = await jsModuleTask;
            await jsModule.InvokeAsync<string>("signIn", BlazorServerIdentityApi.SignInEndpointUrl, ticketValue, properties?.IsPersistent ?? false);

            return;
        }

        await base.HandleSignInAsync(user, properties);
    }

    protected override async Task HandleSignOutAsync(AuthenticationProperties? properties)
    {
        if (IsComponentHubConnection())
        {
            ExtractBlazorConstructs(properties, out var authStateProvider, out var jsRuntime);

            var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity());
            var authState = new AuthenticationState(claimsPrincipal);
            authStateProvider.SetAuthenticationState(Task.FromResult(authState));

            // Initiate request from client to endpoint to sign-out via cookie
            var jsModuleTask = jsRuntime.InvokeAsync<IJSObjectReference>("import", "./_content/BlazorIdentity.Server/BlazorIdentity.js");
            var jsModule = await jsModuleTask;
            await jsModule.InvokeAsync<string>("signOut", BlazorServerIdentityApi.SignOutEndpointUrl);

            return;
        }

        await base.HandleSignOutAsync(properties);
    }

    private bool IsComponentHubConnection() => Context.GetEndpoint()?.Metadata.GetMetadata<HubMetadata>()?.HubType.FullName == "Microsoft.AspNetCore.Components.Server.ComponentHub";

    private void ExtractBlazorConstructs(AuthenticationProperties? properties, out IHostEnvironmentAuthenticationStateProvider authStateProvider, out IJSRuntime jsRuntime)
    {
        if (properties?.Parameters.TryGetValue(nameof(IHostEnvironmentAuthenticationStateProvider), out var authenticationStateProviderParameter) != true
            || !(authenticationStateProviderParameter is IHostEnvironmentAuthenticationStateProvider authStateProviderValue))
        {
            throw new InvalidOperationException("Could not find IHostEnvironmentAuthenticationStateProvider in AuthenticationProperties.");
        }

        if (properties?.Parameters.TryGetValue(nameof(IJSRuntime), out var jsRuntimeParameter) != true
            || !(jsRuntimeParameter is IJSRuntime jsRuntimeValue))
        {
            throw new InvalidOperationException("Could not find IJSRuntime in AuthenticationProperties.");
        }

        authStateProvider = authStateProviderValue;
        jsRuntime = jsRuntimeValue;
    }

    private string? GetTlsTokenBinding()
    {
        var binding = Context.Features.Get<ITlsTokenBindingFeature>()?.GetProvidedTokenBindingId();
        return binding == null ? null : Convert.ToBase64String(binding);
    }
}
