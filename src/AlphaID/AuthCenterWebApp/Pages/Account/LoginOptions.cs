namespace AuthCenterWebApp.Pages.Account;

/// <summary>
///     登录选项。
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
    /// 记住用户登录最长天数。
    /// </summary>
    public int RememberMeLoginDuration { get; set; } = 30;

    /// <summary>
    /// 错误凭据时的消息。
    /// </summary>
    public string InvalidCredentialsErrorMessage { get; set; } = "用户名或密码无效";
}