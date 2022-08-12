using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using Identity = Microsoft.AspNetCore.Identity;

namespace BlazorIdentity.Server;

internal class BlazorServerUserManager<TUser> : IBlazorUserManager<TUser> where TUser : class
{
    private readonly Identity.UserManager<TUser> _userManager;
    private readonly Identity.IdentityOptions _identityOptions;

    public BlazorServerUserManager(Identity.UserManager<TUser> userManager)
    {
        _userManager = userManager;
        _identityOptions = userManager.Options;

        Options = new() { SignIn = new() { RequireConfirmedAccount = _identityOptions.SignIn.RequireConfirmedAccount } };
    }

    public bool SupportsUserEmail => _userManager.SupportsUserEmail;

    public IdentityOptions Options { get; }

    public async Task<IList<UserLoginInfo>> GetLoginsAsync(TUser user) =>
        ToBlazorUserLoginInfoResult((List<Identity.UserLoginInfo>) await _userManager.GetLoginsAsync(user));

    public Task<bool> HasPasswordAsync(TUser user) => _userManager.HasPasswordAsync(user);

    public Task<bool> IsEmailConfirmedAsync(TUser user) => _userManager.IsEmailConfirmedAsync(user);

    public async Task<IdentityResult> ResetPasswordAsync(TUser user, string code, string password)
        => ToBlazorIdentityResult(await _userManager.ResetPasswordAsync(user, code, password));

    public async Task<IdentityResult> DeleteAsync(TUser user) 
        => ToBlazorIdentityResult(await _userManager.DeleteAsync(user));

	public async Task<IdentityResult> CreateAsync(TUser user, string password)
        => ToBlazorIdentityResult(await _userManager.CreateAsync(user, password));

    public Task<TUser?> GetUserAsync(ClaimsPrincipal principal) => _userManager.GetUserAsync(principal);
    public Task<string?> GetAuthenticatorKeyAsync(TUser user) => _userManager.GetAuthenticatorKeyAsync(user);
	public Task<TUser?> FindByEmailAsync(string email) => _userManager.FindByEmailAsync(email);

    public Task<TUser?> FindByIdAsync(string userId) => _userManager.FindByIdAsync(userId);

    public async Task<IdentityResult> ConfirmEmailAsync(TUser user, string code) 
        => ToBlazorIdentityResult(await _userManager.ConfirmEmailAsync(user, code));

	public Task<string> GetUserIdAsync(TUser user) => _userManager.GetUserIdAsync(user);

    public Task<string> GeneratePasswordResetTokenAsync(TUser user) => _userManager.GeneratePasswordResetTokenAsync(user);

	public Task<string?> GetUserEmailAsync(TUser user) => _userManager.GetEmailAsync(user);

    public Task<string> GenerateEmailConfirmationTokenAsync(TUser user) => _userManager.GenerateEmailConfirmationTokenAsync(user);

    public Task<string> GenerateChangeEmailTokenAsync(TUser user, string email) => _userManager.GenerateChangeEmailTokenAsync(user, email);

	public Task<string?> GetUserNameAsync(TUser user) => _userManager.GetUserNameAsync(user);

    public Task<string?> GetPhoneNumberAsync(TUser user) => _userManager.GetPhoneNumberAsync(user);

    public async Task<IdentityResult> SetPhoneNumberAsync(TUser user, string? phoneNumber)
        => ToBlazorIdentityResult(await _userManager.SetPhoneNumberAsync(user, phoneNumber));

    public async Task<IdentityResult> ChangeEmailAsync(TUser user, string email, string code)
        => ToBlazorIdentityResult(await _userManager.ChangeEmailAsync(user, email, code));

    public async Task<IdentityResult> ChangeUserNameAsync(TUser user, string userName)
        => ToBlazorIdentityResult(await _userManager.SetUserNameAsync(user, userName));

	public async Task<IdentityResult> ChangePasswordAsync(TUser user, string currentPassword, string newPassword)
        => ToBlazorIdentityResult(await _userManager.ChangePasswordAsync(user, currentPassword, newPassword));

    public Dictionary<string, string> GetPersonalData(TUser user)
    {
        var personalData = new Dictionary<string, string>();
        var personalDataProps = typeof(TUser).GetProperties().Where(
            prop => Attribute.IsDefined(prop, typeof(PersonalDataAttribute)));

        foreach (var prop in personalDataProps)
        {
            personalData.Add(prop.Name, prop.GetValue(user)?.ToString() ?? string.Empty);
        }
        return personalData;
    }

    private static IdentityResult ToBlazorIdentityResult(Identity.IdentityResult result)
    {
        return result.Succeeded switch
        {
            true => IdentityResult.Success,
            _ => IdentityResult.Failed(result.Errors.Select(e => new IdentityError { Code = e.Code, Description = e.Description }))
        };
    }

    private static IList<UserLoginInfo> ToBlazorUserLoginInfoResult(List<Identity.UserLoginInfo> results)
    {
        var userLoginInfos = new List<UserLoginInfo>();
        foreach (var result in results)
        {
            var userLoginInfo = new UserLoginInfo
            {
                LoginProvider = result.LoginProvider,
                ProviderDisplayName = result.ProviderDisplayName!,
                ProviderKey = result.ProviderKey
            };
            userLoginInfos.Add(userLoginInfo);
        }
        return userLoginInfos;
    }
}
