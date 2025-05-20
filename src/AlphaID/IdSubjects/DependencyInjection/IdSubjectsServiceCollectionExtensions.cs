using IdSubjects;
using IdSubjects.SecurityAuditing;
using IdSubjects.Validators;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection.Extensions;

#pragma warning disable IDE0130 // 命名空间与文件夹结构不匹配
// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection;
#pragma warning restore IDE0130 // 命名空间与文件夹结构不匹配

/// <summary>
/// Extensions for IdSubjects service injection.
/// </summary>
public static class IdSubjectsServiceCollectionExtensions
{
    /// <summary>
    /// 添加IdSubjects基本组件。部包括身份验证部分。
    /// </summary>
    /// <param name="services"></param>
    /// <param name="setupAction"></param>
    /// <returns></returns>
    public static IdentityBuilder AddIdSubjects<TUser>(this IServiceCollection services,
        Action<IdentityOptions>? setupAction = null)
        where TUser : ApplicationUser
    {
        // 由IdSubjects使用的服务。
        services.TryAddScoped<ApplicationUserManager<TUser>>();
        services.TryAddScoped<PasswordHistoryManager<TUser>>();
        services.TryAddScoped<ProfileUrlGenerator<TUser>>();


        services.TryAddScoped<IEventService, DefaultEventService>();
        services.TryAddScoped<IEventSink, DefaultEventSink>();

        //添加基础标识
        IdentityBuilder builder = services.AddIdentityCore<TUser>()
                .AddUserManager<ApplicationUserManager<TUser>>() //当做UserManager<TUser>使用
                .AddUserValidator<PhoneNumberValidator<TUser>>()
                .AddErrorDescriber<ApplicationUserIdentityErrorDescriber>();

        // 移除原有的PasswordValidator，添加AlphaIdPasswordValidator
        services.RemoveAll<IPasswordValidator<TUser>>(); // 移除所有的PasswordValidator
        builder.AddPasswordValidator<IdSubjectsPasswordValidator<TUser>>();

        if (setupAction != null)
        {
            services.Configure(setupAction);
        }

        return builder;
    }
}