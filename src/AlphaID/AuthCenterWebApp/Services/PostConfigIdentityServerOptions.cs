using AlphaIdPlatform.Platform;
using Duende.IdentityServer.Configuration;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace AuthCenterWebApp.Services;

public class PostConfigIdentityServerOptions(IServiceProvider provider, ILogger<PostConfigIdentityServerOptions> logger) : IPostConfigureOptions<IdentityServerOptions>, IPostConfigureOptions<CookieAuthenticationOptions>
{
    public void PostConfigure(string? name, IdentityServerOptions options)
    {
        using var scope = provider.CreateScope();
        var verificationCodeService = scope.ServiceProvider.GetService<IVerificationCodeService>();
        if (verificationCodeService is null)
        {
            logger.LogWarning("没有为系统配置短信验证码服务，将优先使用用户名密码登录模式。");
        }
        else
        {
            //如果验证码服务不为空，则优先采用登录注册一体流程。
            options.UserInteraction.LoginUrl = "/Account/SignInOrSignUp";
        }
        
    }

    public void PostConfigure(string? name, CookieAuthenticationOptions options)
    {
        using var scope = provider.CreateScope();
        var verificationCodeService = scope.ServiceProvider.GetService<IVerificationCodeService>();
        if (verificationCodeService is null)
        {
            logger.LogWarning("没有为系统配置短信验证码服务，将优先使用用户名密码登录模式。");
        }
        else
        {
            //如果验证码服务不为空，则优先采用登录注册一体流程。
            options.LoginPath = "/Account/SignInOrSignUp";
        }
    }
}
