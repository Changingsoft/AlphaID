using Microsoft.AspNetCore.Identity;

namespace IdSubjects;

/// <summary>
/// 自然人登录结果。继承自<see cref="SignInResult"></see>
/// </summary>
public class ApplicationUserSignInResult:SignInResult
{
    /// <summary>
    /// 指示用户必须更改密码。
    /// </summary>
    public bool MustChangePassword { get; set; }
}
