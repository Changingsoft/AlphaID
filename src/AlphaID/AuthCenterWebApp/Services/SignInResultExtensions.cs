using Microsoft.AspNetCore.Identity;

namespace AuthCenterWebApp.Services;

public static class SignInResultExtensions
{
    public static bool MustChangePassword(this SignInResult signInResult)
    {
        if (signInResult is not PersonSignInResult result)
            return false;
        return result.MustChangePassword;
    }
}
