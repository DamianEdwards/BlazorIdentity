namespace BlazorIdentity.ServerApp.Data;

using Microsoft.AspNetCore.Identity;

public class AppUserLoginInfo : UserLoginInfo
{
	public AppUserLoginInfo(string loginProvider, string providerKey, string? displayName) : base(loginProvider, providerKey, displayName)
	{
	}
}
