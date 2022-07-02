using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Options;

namespace Microsoft.AspNetCore.Builder;

public static class BlazorServerIdentityApi
{
    private const string identityApiGroupPrefix = $"/_{nameof(BlazorServerIdentityApi)}/Account";
    private const string signInEndpointPath = "/SignIn";
    private const string signOutEndpointPath = "/SignOut";

    internal const string SignInEndpointUrl = $"{identityApiGroupPrefix}{signInEndpointPath}";
    internal const string SignOutEndpointUrl = $"{identityApiGroupPrefix}{signOutEndpointPath}";

    /// <summary>
    /// Maps API endpoints for to support Blazor Identity in Blazor Server apps.
    /// </summary>
    /// <typeparam name="TUser">The user type.</typeparam>
    /// <param name="builder">The <see cref="IEndpointRouteBuilder"/>.</param>
    /// <returns>The <see cref="IEndpointRouteBuilder"/>.</returns>
    public static RouteGroupBuilder MapBlazorServerIdentityApi<TUser>(this IEndpointRouteBuilder builder) where TUser : class
    {
        var group = builder.MapGroup(identityApiGroupPrefix);

        group.ExcludeFromDescription();

        group.MapPost(signInEndpointPath,
            async (
                GetAuthenticationCookieRequest request,
                HttpContext httpContext,
                UserManager<TUser> userManager,
                SignInManager<TUser> signInManager,
                IOptionsMonitor<CookieAuthenticationOptions> cookieAuthnOptions) =>
            {
                var options = cookieAuthnOptions.Get(IdentityConstants.ApplicationScheme);

                // Validate ticket
                var ticketValue = request.Ticket;
                if (string.IsNullOrEmpty(ticketValue))
                {
                    return Results.BadRequest("Missing ticket value");
                }

                // TODO: Can we use TLS token binding as a purpose here? Concerned that the circuit and JS fetch calls will actually
                //       be over different TLS connections as HTTP/1.1 WebSocket uses a dedicated connection.
                var ticket = options.TicketDataFormat.Unprotect(ticketValue);
                if (ticket is null)
                {
                    return Results.BadRequest("Ticket value was invalid");
                }

                // Get user and sign-in
                var user = await userManager.GetUserAsync(ticket.Principal);
                if (user is null)
                {
                    return Results.BadRequest("Error signing in");
                }

                await signInManager.SignInAsync(user, request.Persist);

                return Results.Ok();
            });

        group.MapPost(signOutEndpointPath, async (ClaimsPrincipal user, SignInManager<TUser> signInManager) =>
            {
                if (signInManager.IsSignedIn(user))
                {
                    await signInManager.SignOutAsync();
                }

                return Results.Ok();
            });

        return group;
    }

    private static string? GetTlsTokenBinding(HttpContext context)
    {
        var binding = context.Features.Get<ITlsTokenBindingFeature>()?.GetProvidedTokenBindingId();
        return binding == null ? null : Convert.ToBase64String(binding);
    }
}

internal class GetAuthenticationCookieRequest
{
    public string? Ticket { get; set; }
    public bool Persist { get; set; }
}