using Microsoft.AspNetCore.Authentication;

namespace AuthCenterWebApp.Services.WechatMp;

public static class WechatMpAuthExtensions
{
    /// <summary>
    /// Adds WeChat Media Platform authentication to the specified <see cref="AuthenticationBuilder"/>.
    /// </summary>
    /// <remarks>This method extends the <see cref="AuthenticationBuilder"/> to support authentication using
    /// WeChat Media Platform. It configures the authentication scheme with the specified options and handler.</remarks>
    /// <param name="builder">The <see cref="AuthenticationBuilder"/> to which the WeChat Media Platform authentication is added.</param>
    /// <param name="authenticationScheme">The authentication scheme name used to identify the handler.</param>
    /// <param name="displayName">The display name for the authentication scheme.</param>
    /// <param name="config">A delegate to configure the <see cref="WechatMpAuthOptions"/>.</param>
    /// <returns>The <see cref="AuthenticationBuilder"/> with the WeChat Media Platform authentication added.</returns>
    public static AuthenticationBuilder AddWechatMp(this AuthenticationBuilder builder, string authenticationScheme, string displayName, Action<WechatMpAuthOptions> config)
    {
        return builder.AddOAuth<WechatMpAuthOptions, WechatMpOAuthHandler>(authenticationScheme, displayName, config);
    }
}
