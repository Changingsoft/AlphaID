using IdSubjects;
using IdSubjects.Identity;
using IdSubjects.SecurityAuditing;
using IdSubjects.Validators;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection.Extensions;

#pragma warning disable IDE0130 // 命名空间与文件夹结构不匹配
namespace Microsoft.Extensions.DependencyInjection;
#pragma warning restore IDE0130 // 命名空间与文件夹结构不匹配

/// <summary>
/// 
/// </summary>
public static class IdSubjectsIdentityServiceCollectionExtensions
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TUser"></typeparam>
    /// <typeparam name="TRole"></typeparam>
    /// <param name="services"></param>
    /// <param name="setupAction"></param>
    /// <returns></returns>
    public static IdentityBuilder AddIdSubjectsIdentity<TUser, TRole>(this IServiceCollection services, Action<IdentityOptions>? setupAction = null)
        where TUser : ApplicationUser
        where TRole : class
    {
        IdentityBuilder builder;
        if (setupAction == null)
            builder = services.AddIdentity<TUser, TRole>();
        else
            builder = services.AddIdentity<TUser, TRole>(setupAction);

        builder.AddUserManager<ApplicationUserManager<TUser>>()
            .AddSignInManager<ApplicationUserSignInManager<TUser>>()
            .AddClaimsPrincipalFactory<ApplicationUserClaimsPrincipalFactory<TUser, TRole>>()
            .AddUserValidator<PhoneNumberValidator<TUser>>()
            .AddErrorDescriber<ApplicationUserIdentityErrorDescriber>();

        // 移除原有的PasswordValidator，添加AlphaIdPasswordValidator
        services.RemoveAll<IPasswordValidator<TUser>>(); // 移除所有的PasswordValidator
        builder.AddPasswordValidator<AlphaIdPasswordValidator<TUser>>();

        services.AddScoped<PasswordHistoryManager<TUser>>();
        services.AddScoped<IEventService, WebAppEventService>();
        services.TryAddScoped<IEventSink, DefaultEventSink>();
        services.TryAddScoped<ProfileUrlGenerator<TUser>>();

        services.AddAuthentication().AddCookie(IdSubjectsIdentityDefaults.MustChangePasswordScheme, o =>
        {
            o.Cookie.Name = IdSubjectsIdentityDefaults.MustChangePasswordScheme;
            o.ExpireTimeSpan = TimeSpan.FromMinutes(5);
        });

        return builder;
    }
}
