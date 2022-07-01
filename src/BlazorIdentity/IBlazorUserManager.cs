namespace BlazorIdentity;

public interface IBlazorUserManager<TUser> where TUser : class
{
    bool SupportsUserEmail { get; }
    IdentityOptions Options { get; }
    Task<IdentityResult> CreateAsync(TUser user, string password);
    Task<string> GetUserIdAsync(TUser user);
}
