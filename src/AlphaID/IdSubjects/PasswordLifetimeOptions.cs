namespace IdSubjects;

/// <summary>
///     密码选项。
/// </summary>
public class PasswordLifetimeOptions
{
    /// <summary>
    ///     是否启用密码过期。默认为false。
    /// </summary>
    public bool EnablePassExpires { get; set; } = false;

    /// <summary>
    ///     密码过期时间，默认为90天。
    /// </summary>
    public int PasswordExpiresDay { get; set; } = 90;

    /// <summary>
    ///     记住密码历史数量。当用户重设密码时，不能设为已计入历史的密码。默认为0。意味着用户可以重设相同的密码。
    /// </summary>
    public int RememberPasswordHistory { get; set; } = 0;

    /// <summary>
    ///     密码最小寿命（分钟），用户自上一次设置密码以后必须经过最小寿命才能再次重设密码。默认为0，即无最下寿命。
    /// </summary>
    public int MinimumAge { get; set; } = 0;
}