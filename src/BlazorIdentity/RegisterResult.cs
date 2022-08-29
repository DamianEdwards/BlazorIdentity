namespace BlazorIdentity;

public class RegisterResult
{
    public bool RequireConfirmedAccount { get; set; } = false;
    public string ReturnUrl { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}
