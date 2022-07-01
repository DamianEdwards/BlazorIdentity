namespace BlazorIdentity.Abstractions;

public class IdentityOptions
{
    public SignInOptions SignIn { get; init; } = default!;
}
