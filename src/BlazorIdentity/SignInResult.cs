namespace BlazorIdentity;

public class SignInResult
{
    private static readonly SignInResult _success = new SignInResult { Succeeded = true };
    private static readonly SignInResult _failed = new SignInResult();
    private static readonly SignInResult _lockedOut = new SignInResult { IsLockedOut = true };
    private static readonly SignInResult _notAllowed = new SignInResult { IsNotAllowed = true };
    private static readonly SignInResult _twoFactorRequired = new SignInResult { RequiresTwoFactor = true };

    public bool Succeeded { get; set; }
    public bool IsLockedOut { get; set; }
    public bool IsNotAllowed { get; set; }
    public bool RequiresTwoFactor { get; set; }
    public string ReturnUrl { get; set; } = string.Empty;

    public static SignInResult Success => _success;
    public static SignInResult Failed => _failed;
    public static SignInResult LockedOut => _lockedOut;
    public static SignInResult TwoFactorRequired => _twoFactorRequired;
    public override string ToString()
    {
        return IsLockedOut ? "Lockedout" :
                  IsNotAllowed ? "NotAllowed" :
               RequiresTwoFactor ? "RequiresTwoFactor" :
               Succeeded ? "Succeeded" : "Failed";
    }
}
