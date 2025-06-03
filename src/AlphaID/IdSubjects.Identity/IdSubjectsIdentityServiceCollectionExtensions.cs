using IdSubjects;
using IdSubjects.Identity;
using IdSubjects.SecurityAuditing;
using IdSubjects.Validators;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection.Extensions;

#pragma warning disable IDE0130 // 命名空间与文件夹结构不匹配
// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection;
#pragma warning restore IDE0130 // 命名空间与文件夹结构不匹配

/// <summary>
/// IdSubjects.Identity的扩展方法。
/// </summary>
public static class IdSubjectsIdentityServiceCollectionExtensions
{
    /// <summary>
    /// 添加IdSubjects Identity服务。包括身份验证。
    /// </summary>
    /// <typeparam name="TUser">表示用户类型。</typeparam>
    /// <typeparam name="TRole">表示角色的类型。</typeparam>
    /// <param name="services"></param>
    /// <param name="setupAction"></param>
    /// <returns></returns>
    public static IdentityBuilder AddIdSubjectsIdentity<TUser, TRole>(this IServiceCollection services, Action<IdentityOptions>? setupAction = null)
        where TUser : ApplicationUser
        where TRole : class
    {
        IdentityBuilder builder = setupAction == null ? services.AddIdentity<TUser, TRole>() : services.AddIdentity<TUser, TRole>(setupAction);

        builder.AddUserManager<ApplicationUserManager<TUser>>()
            .AddSignInManager<ApplicationUserSignInManager<TUser>>()
            .AddClaimsPrincipalFactory<ApplicationUserClaimsPrincipalFactory<TUser, TRole>>()
            .AddUserValidator<PhoneNumberValidator<TUser>>()
            .AddErrorDescriber<ApplicationUserIdentityErrorDescriber>();

        // 移除原有的PasswordValidator，添加AlphaIdPasswordValidator
        services.RemoveAll<IPasswordValidator<TUser>>(); // 移除所有的PasswordValidator
        builder.AddPasswordValidator<IdSubjectsPasswordValidator<TUser>>();

        services.AddScoped<IEventService, WebAppEventService>();
        services.TryAddScoped<IEventSink, DefaultEventSink>();
        services.TryAddScoped<ProfileUrlGenerator<TUser>>();

        //添加MustChangePasswordScheme方案。
        services.AddAuthentication().AddCookie(IdSubjectsIdentityDefaults.MustChangePasswordScheme, o =>
        {
            o.Cookie.Name = IdSubjectsIdentityDefaults.MustChangePasswordScheme;
            o.ExpireTimeSpan = TimeSpan.FromMinutes(5);
        });

        return builder;
    }
}
