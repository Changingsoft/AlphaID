using Microsoft.AspNetCore.Identity;

namespace AuthCenterWebApp.Services;

public class PersonSignInResult : SignInResult
{
    public bool MustChangePassword { get; init; }
}
