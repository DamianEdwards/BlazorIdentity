using System.Security.Claims;

namespace BlazorIdentity;

public interface IBlazorUserStore<TUser> where TUser : class
{
    Task GetUserIdAsync(TUser user, CancellationToken cancellationToken);

    Task SetUserNameAsync(TUser user, string? userName, CancellationToken cancellationToken);
}
