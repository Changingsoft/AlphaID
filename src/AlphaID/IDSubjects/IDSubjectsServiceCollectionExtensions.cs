using IDSubjects;
using IDSubjects.Validators;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Extensions for IDSubjects service injection.
/// </summary>
public static class IDSubjectsServiceCollectionExtensions
{

    /// <summary>
    /// 向基础设施添加AlphaID自然人标识管理功能。
    /// </summary>
    /// <param name="services"></param>
    /// <param name="setupAction"></param>
    /// <returns></returns>
    public static IdentityBuilder AddIdSubjectsIdentityCore(this IServiceCollection services, Action<IdentityOptions> setupAction)
    {
        // 由IDSubjects使用的服务。
        services.TryAddScoped<OrganizationManager>();
        services.TryAddScoped<OrganizationMemberManager>();
        services.TryAddScoped<OrganizationSearcher>();

        //添加基础标识
        var builder = services.AddIdentityCore<NaturalPerson>(setupAction)
            .AddUserManager<NaturalPersonManager>()
            //.AddUserValidator<PhoneNumberValidator>()
            //.AddErrorDescriber<NaturalPersonIdentityErrorDescriber>()
            ;
        return builder;
    }

    /// <summary>
    /// 向基础设施添加自然人标识管理功能，并设置通过自然人标识来处理用户身份验证。
    /// </summary>
    /// <param name="services"></param>
    /// <param name="setupAction"></param>
    /// <returns></returns>
    public static IdentityBuilder AddIdSubjectsIdentity(this IServiceCollection services, Action<IdentityOptions> setupAction)
    {
        var builder = AddIdSubjectsIdentityCore(services, setupAction);
        //Add required cookies.
        services.AddAuthentication(options =>
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
            .AddCookie(IDSubjectsIdentityDefaults.MustChangePasswordScheme, o =>
            {
                o.Cookie.Name = IDSubjectsIdentityDefaults.MustChangePasswordScheme;
                o.ExpireTimeSpan = TimeSpan.FromMinutes(5);
            });
        return builder;
    }

}
