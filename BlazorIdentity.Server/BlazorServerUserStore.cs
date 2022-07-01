using Identity = Microsoft.AspNetCore.Identity;
using BlazorIdentity.Abstractions;
using Microsoft.AspNetCore.Identity;

namespace BlazorIdentity.Server;

internal class BlazorServerUserStore<TUser> : IBlazorUserStore<TUser> where TUser : class
{
    private readonly IUserStore<TUser> _userStore;

    public BlazorServerUserStore(Identity.IUserStore<TUser> userStore)
    {
        _userStore = userStore;
    }

    public Task GetUserIdAsync(TUser user, CancellationToken cancellationToken)
    {
        return _userStore.GetUserIdAsync(user, cancellationToken);
    }

    public Task SetUserNameAsync(TUser user, string? userName, CancellationToken cancellationToken)
    {
        return _userStore.SetUserNameAsync(user, userName, cancellationToken);
    }
}
