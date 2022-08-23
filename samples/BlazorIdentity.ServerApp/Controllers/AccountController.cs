namespace BlazorIdentity.ServerApp.Controllers;

using BlazorIdentity.ServerApp.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Text.Json.Serialization;

[AllowAnonymous]
[ApiController]
[Route("api/[controller]/[action]")]

public partial class AccountController : ControllerBase
{
    private readonly IBlazorSignInManager<AppUser> _signInManager;

    private readonly IBlazorUserManager<AppUser> _userManager;
    public string ErrorMessage { get; set; } = string.Empty;

    public AccountController(IBlazorSignInManager<AppUser> signInManager, IBlazorUserManager<AppUser> userManager)
    {
        _signInManager = signInManager;
        _userManager = userManager;
    }

    [HttpPost]
    public IActionResult ExternalLogin([FromForm] string provider, string returnUrl)
    {
        var redirectUrl = $"/account/ExternalLoginCallback?returnUrl={returnUrl}&provider={provider}";

        var properties = new AuthenticationProperties
        {
            RedirectUri = redirectUrl
        };
        properties.Items["LoginProvider"] = provider;

        return Challenge(properties, provider);
    }

    [HttpGet]
    public async Task<IActionResult> GetExternalLoginInfo()
    {
        var info = await _signInManager.GetExternalLoginInfoAsync();
        return Ok(JsonSerializer.Serialize(info));
    }
}
