using AlphaIdPlatform.Identity;
using IdSubjects.DependencyInjection;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// </summary>
public static class IdSubjectsServiceCollectionExtensions
{
    /// <summary>
    ///     向基础设施添加自然人标识管理功能，并设置通过自然人标识来处理用户身份验证。
    /// </summary>
    /// <param name="services"></param>
    /// <param name="setupAction"></param>
    /// <returns></returns>
    public static IdSubjectsIdentityBuilder AddIdSubjectsIdentity(this IServiceCollection services,
        Action<IdSubjectsOptions>? setupAction = null)
    {
        services.AddHttpContextAccessor();

        IdSubjectsBuilder idSubjectsBuilder = services.AddIdSubjects(setupAction);

        //Add required cookies.
        AuthenticationBuilder authenticationBuilder = services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = IdentityConstants.ApplicationScheme;
                options.DefaultChallengeScheme = IdentityConstants.ApplicationScheme;
                options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
            })
            .AddCookie(IdentityConstants.ApplicationScheme, o =>
            {
                o.LoginPath = new PathString("/Account/Login");
                o.Events = new CookieAuthenticationEvents
                {
                    OnValidatePrincipal = SecurityStampValidator.ValidatePrincipalAsync
                };
            })
            .AddCookie(IdentityConstants.ExternalScheme, o =>
            {
                o.Cookie.Name = IdentityConstants.ExternalScheme;
                o.ExpireTimeSpan = TimeSpan.FromMinutes(5);
            })
            .AddCookie(IdentityConstants.TwoFactorRememberMeScheme, o =>
            {
                o.Cookie.Name = IdentityConstants.TwoFactorRememberMeScheme;
                o.Events = new CookieAuthenticationEvents
                {
                    OnValidatePrincipal = SecurityStampValidator.ValidateAsync<ITwoFactorSecurityStampValidator>
                };
            })
            .AddCookie(IdentityConstants.TwoFactorUserIdScheme, o =>
            {
                o.Cookie.Name = IdentityConstants.TwoFactorUserIdScheme;
                o.ExpireTimeSpan = TimeSpan.FromMinutes(5);
            })
            //添加必须修改密码cookie。
            .AddCookie(IdSubjectsIdentityDefaults.MustChangePasswordScheme, o =>
            {
                o.Cookie.Name = IdSubjectsIdentityDefaults.MustChangePasswordScheme;
                o.ExpireTimeSpan = TimeSpan.FromMinutes(5);
            });
        return new IdSubjectsIdentityBuilder(services, idSubjectsBuilder, authenticationBuilder) ;
    }
}