namespace AuthCenterWebApp;

/// <summary>
/// 登录选项。
/// </summary>
public class LoginOptions
{
    /// <summary>
    /// 允许本地登录
    /// </summary>
    public bool AllowLocalLogin { get; set; } = true;

    /// <summary>
    /// 允许记住用户登录。
    /// </summary>
    public bool AllowRememberLogin { get; set; } = true;

    /// <summary>
    /// 允许快速注册。当验证码服务可用时，启用快速注册功能，用户可以在登录界面直接注册新账号。
    /// </summary>
    public bool EnableQuickSignUp { get; set; } = true;
}