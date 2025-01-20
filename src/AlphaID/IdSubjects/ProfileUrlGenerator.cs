using Microsoft.Extensions.Options;

namespace IdSubjects;

/// <summary>
/// 一个用于生成用户个人资料URL的类。
/// </summary>
/// <typeparam name="T"></typeparam>
/// <param name="options"></param>
public class ProfileUrlGenerator<T>(IOptions<OidcProfileUrlOptions> options)
    where T : ApplicationUser
{
    /// <summary>
    /// 
    /// </summary>
    protected OidcProfileUrlOptions Options { get; } = options.Value;

    /// <summary>
    /// 生成用户个人资料URL。
    /// </summary>
    /// <param name="user"></param>
    /// <returns></returns>
    public virtual Uri GenerateProfileUrl(T user)
    {
        return new Uri(options.Value.ProfileUrlBase, $"/User/{user.UserName}");
    }

    /// <summary>
    /// 生成用户个人资料图片URL。
    /// </summary>
    /// <param name="user"></param>
    /// <returns></returns>
    public virtual Uri GenerateProfilePictureUrl(T user)
    {
        return new Uri(options.Value.ProfileUrlBase, $"/User/ProfilePicture/{user.UserName}");
    }
}
