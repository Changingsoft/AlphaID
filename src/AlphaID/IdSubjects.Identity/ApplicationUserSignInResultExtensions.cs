using Microsoft.AspNetCore.Identity;

namespace IdSubjects.Identity;

/// <summary>
///    自然人登录结果扩展。
/// </summary>
public static class ApplicationUserSignInResultExtensions
{
    /// <summary>
    /// 获取一个值，指示用户必须更改其密码。
    /// </summary>
    /// <param name="signInResult"></param>
    /// <returns></returns>
    public static bool MustChangePassword(this SignInResult signInResult)
    {
        if (signInResult is not ApplicationUserSignInResult result)
            return false;
        return result.MustChangePassword;
    }
}