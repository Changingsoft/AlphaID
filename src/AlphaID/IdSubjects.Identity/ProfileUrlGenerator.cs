using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace IdSubjects.Identity;

/// <summary>
/// 一个用于生成用户个人资料URL的类。
/// </summary>
/// <typeparam name="T"></typeparam>
/// <param name="accessor"></param>
public class ProfileUrlGenerator<T>(IHttpContextAccessor accessor)
    where T : ApplicationUser
{
    /// <summary>
    /// Gets the current HTTP context for the request.
    /// </summary>
    protected HttpContext Context => accessor.HttpContext ?? throw new InvalidOperationException("HttpContext is not available.");

    /// <summary>
    /// 生成用户个人资料URL。
    /// </summary>
    /// <param name="user"></param>
    /// <returns></returns>
    public virtual Uri GenerateProfileUrl(T user)
    {
        var baseUrl = new Uri($"{Context.Request.Scheme}://{Context.Request.Host}");
        return new Uri(baseUrl, $"/User/{user.UserName}");
    }

    /// <summary>
    /// 生成用户个人资料图片URL。
    /// </summary>
    /// <param name="user"></param>
    /// <returns></returns>
    public virtual Uri GenerateProfilePictureUrl(T user)
    {
        var baseUrl = new Uri($"{Context.Request.Scheme}://{Context.Request.Host}");
        return new Uri(baseUrl, $"/User/ProfilePicture/{user.UserName}");
    }
}
