namespace BlazorIdentity;

using System.Collections.Generic;
using System.Security.Claims;

public class ExternalLoginInfo
{
    public IEnumerable<AuthToken> AuthenticationTokens { get; set; } = new List<AuthToken>();
    public ClaimsPrincipal Principal { get; set; }

    public string LoginProviderName { get; set; }
    public string ProviderKey { get; set; }
    public string DisplayName { get; set; }

    public ExternalLoginInfo(ClaimsPrincipal principal, string loginProviderName, string providerKey, string displayName)
    {
        Principal = principal;
        LoginProviderName = loginProviderName;
        ProviderKey = providerKey;
        DisplayName = displayName;
    }
}
