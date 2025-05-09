using IdSubjects;
using IdSubjects.DependencyInjection;
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
    /// 向添加AlphaId自然人管理功能。
    /// </summary>
    /// <param name="services"></param>
    /// <param name="setupAction"></param>
    /// <returns></returns>
    public static IdSubjectsBuilder AddIdSubjects<TUser>(this IServiceCollection services,
        Action<IdentityOptions>? setupAction = null)
        where TUser : ApplicationUser
    {
        services.AddHttpContextAccessor();

        // 由IdSubjects使用的服务。
        services.TryAddScoped<ApplicationUserManager<TUser>>();
        services.TryAddScoped<ApplicationUserIdentityErrorDescriber>();
        services.TryAddScoped<PasswordHistoryManager<TUser>>();



        services.TryAddScoped<IEventService, DefaultEventService>();
        services.TryAddScoped<IEventSink, DefaultEventSink>();

        //添加基础标识
        IdentityBuilder identityBuilder = services.AddIdentityCore<TUser>()
                .AddUserManager<ApplicationUserManager<TUser>>() //当做UserManager<T>使用
                .AddSignInManager<ApplicationUserSignInManager<TUser>>()
                .AddUserValidator<PhoneNumberValidator<TUser>>()
                .AddDefaultTokenProviders();

        // 移除原有的PasswordValidator
        var passwordValidatorDescriptor = services.FirstOrDefault(d => d.ServiceType == typeof(IPasswordValidator<TUser>));
        if (passwordValidatorDescriptor != null)
        {
            services.Remove(passwordValidatorDescriptor);
        }
        // 添加AlphaIdPasswordValidator
        identityBuilder.AddPasswordValidator<AlphaIdPasswordValidator<TUser>>();

        if (setupAction != null)
        {
            services.Configure(setupAction);
        }

        return new IdSubjectsBuilder(services, identityBuilder);
    }

    /// <summary>
    /// 添加IdSubjects的Identity服务，包括身份验证功能。
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
        var builder = services.AddIdentity<TUser, TRole>()
            .AddUserManager<ApplicationUserManager<TUser>>()
            .AddUserValidator<PhoneNumberValidator<TUser>>();

        // 移除原有的PasswordValidator
        var passwordValidatorDescriptor = services.FirstOrDefault(d => d.ServiceType == typeof(IPasswordValidator<TUser>));
        if (passwordValidatorDescriptor != null)
        {
            services.Remove(passwordValidatorDescriptor);
        }
        // 添加AlphaIdPasswordValidator
        builder.AddPasswordValidator<AlphaIdPasswordValidator<TUser>>();

        services.AddAuthentication().AddCookie(IdSubjectsIdentityDefaults.MustChangePasswordScheme, o =>
        {
            o.Cookie.Name = IdSubjectsIdentityDefaults.MustChangePasswordScheme;
            o.ExpireTimeSpan = TimeSpan.FromMinutes(5);
        });
        if (setupAction != null)
        {
            services.Configure(setupAction);
        }
        return builder;
    }
}