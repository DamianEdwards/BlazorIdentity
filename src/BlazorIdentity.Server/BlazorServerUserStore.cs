using Microsoft.AspNetCore.Identity;

namespace BlazorIdentity.Server;

internal class BlazorServerUserStore<TUser> : IBlazorUserStore<TUser>, IBlazorUserEmailStore<TUser> where TUser : class
{
    private readonly IUserStore<TUser> _userStore;
    private readonly IUserEmailStore<TUser>? _emailStore;

    public BlazorServerUserStore(IUserStore<TUser> userStore)
    {
        _userStore = userStore;
        _emailStore = userStore as IUserEmailStore<TUser>;
    }

    public Task GetUserIdAsync(TUser user, CancellationToken cancellationToken)
    {
        return _userStore.GetUserIdAsync(user, cancellationToken);
    }

    public Task SetEmailAsync(TUser user, string? email, CancellationToken cancellationToken)
    {
        if (_emailStore is null)
        {
            throw new NotSupportedException($"{_userStore.GetType().FullName} does not implement {nameof(IUserEmailStore<TUser>)}.");
        }

        return _emailStore.SetEmailAsync(user, email, cancellationToken);
    }

    public Task SetUserNameAsync(TUser user, string? userName, CancellationToken cancellationToken)
    {
        return _userStore.SetUserNameAsync(user, userName, cancellationToken);
    }
}
