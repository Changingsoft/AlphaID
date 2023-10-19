using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;

namespace XinAnAuth;

/// <summary>
/// Extensions for add service feature to AuthenticationBuilder.
/// </summary>
public static class XinAnAuthenticationOptionsExtensions
{
    /// <summary>
    /// 在身份验证中添加信安世纪外部身份验证机制。
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="authenticationScheme"></param>
    /// <param name="displayName"></param>
    /// <param name="configureOptions"></param>
    /// <returns></returns>
    public static AuthenticationBuilder AddXinAn(this AuthenticationBuilder builder, string authenticationScheme, string displayName, Action<XinAnOptions> configureOptions)
            => builder.AddOAuth<XinAnOptions, XinAnHandler>(authenticationScheme, displayName, configureOptions);
}
