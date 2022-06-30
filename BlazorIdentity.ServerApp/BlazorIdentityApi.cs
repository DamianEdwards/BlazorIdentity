using System.Security.Claims;
using BlazorIdentity.ServerApp.Data;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace Microsoft.AspNetCore.Builder;

public static class BlazorIdentityApi
{
    internal const string SignInEndpointUrl = "/Identity/Account/SignIn";
    internal const string SignOutEndpointUrl = "/Identity/Account/SignOut";

    public static IEndpointRouteBuilder MapBlazorIdentity(this IEndpointRouteBuilder routes)
    {
        routes.MapPost(SignInEndpointUrl, async (GetAuthenticationCookieRequest request, HttpContext httpContext, UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, IOptions<CookieAuthenticationOptions> cookieAuthnOptions) =>
            {
                var options = cookieAuthnOptions.Value;

                // Validate ticket
                var ticketValue = request.Ticket;
                if (string.IsNullOrEmpty(ticketValue))
                {
                    return Results.BadRequest();
                }

                var ticket = options.TicketDataFormat.Unprotect(ticketValue);
                if (ticket is null)
                {
                    return Results.BadRequest();
                }

                // Get user and sign-in
                var user = await userManager.GetUserAsync(ticket.Principal);
                if (user is null)
                {
                    return Results.BadRequest();
                }

                await signInManager.SignInAsync(user, request.Persist);

                return Results.Ok();
            })
            .ExcludeFromDescription();

        routes.MapPost(SignOutEndpointUrl, async (ClaimsPrincipal user, SignInManager<AppUser> signInManager) =>
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
}

internal class GetAuthenticationCookieRequest
{
    public string? Ticket { get; set; }
    public bool Persist { get; set; }
}