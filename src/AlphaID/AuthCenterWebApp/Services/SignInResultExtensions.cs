using Microsoft.AspNetCore.Identity;

namespace AuthCenterWebApp.Services;

public static class SignInResultExtensions
{
    /// <summary>
    /// 获取一个值，指示用户必须更改其密码。
    /// </summary>
    /// <param name="signInResult"></param>
    /// <returns></returns>
    public static bool MustChangePassword(this SignInResult signInResult)
    {
        if (signInResult is not PersonSignInResult result)
            return false;
        return result.MustChangePassword;
    }
}
