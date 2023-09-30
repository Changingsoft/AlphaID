using Microsoft.AspNetCore.Authentication;

namespace AuthCenterWebApp.Services.XinAn;

public static class XinAnAuthenticationOptionsExtensions
{

    public static AuthenticationBuilder AddXinAn(this AuthenticationBuilder builder, string authenticationScheme, string displayName, Action<XinAnOptions> configureOptions)
            => builder.AddOAuth<XinAnOptions, XinAnHandler>(authenticationScheme, displayName, configureOptions);
}
