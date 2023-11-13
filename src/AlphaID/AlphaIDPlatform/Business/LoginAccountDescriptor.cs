namespace AlphaIDPlatform.Business;

/// <summary>
/// 登录账号描述子。
/// </summary>
public class LoginAccountDescriptor
{
    /// <summary>
    /// 首选账户名称。
    /// </summary>
    public string PrimaryAccountName { get; set; } = default!;

    /// <summary>
    /// 备选账户名称。
    /// </summary>
    public string SecondaryAccountName { get; set; } = default!;
}
