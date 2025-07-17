using Microsoft.AspNetCore.Authentication;

namespace AuthCenterWebApp.Services.WechatMp;

public static class WechatMpAuthExtensions
{
    public static AuthenticationBuilder AddWechatMp(this AuthenticationBuilder builder, string authenticationScheme, string displayName, Action<WechatMpAuthOptions> config)
    {
        return builder.AddOAuth<WechatMpAuthOptions, WechatMpOAuthHandler>(authenticationScheme, displayName, config);
    }
}
