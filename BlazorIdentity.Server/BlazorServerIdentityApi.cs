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
    internal const string SignInEndpointUrl = "/Identity/Account/SignIn";
    internal const string SignOutEndpointUrl = "/Identity/Account/SignOut";

    public static IEndpointRouteBuilder MapBlazorIdentity<TUser>(this IEndpointRouteBuilder routes) where TUser : class
    {
        routes.MapPost(SignInEndpointUrl,
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
            })
            .ExcludeFromDescription();

        routes.MapPost(SignOutEndpointUrl, async (ClaimsPrincipal user, SignInManager<TUser> signInManager) =>
            {
                if (signInManager.IsSignedIn(user))
                {
                    await signInManager.SignOutAsync();
                }

                return Results.Ok();
            })
            .ExcludeFromDescription();

        return routes;
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