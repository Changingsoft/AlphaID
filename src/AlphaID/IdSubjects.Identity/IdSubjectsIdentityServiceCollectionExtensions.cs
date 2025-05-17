using IdSubjects;
using IdSubjects.Identity;
using IdSubjects.SecurityAuditing;
using IdSubjects.Validators;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection.Extensions;

#pragma warning disable IDE0130 // 命名空间与文件夹结构不匹配
namespace Microsoft.Extensions.DependencyInjection;
#pragma warning restore IDE0130 // 命名空间与文件夹结构不匹配

public static class IdSubjectsIdentityServiceCollectionExtensions
{
    public static IdentityBuilder AddIdSubjectsIdentity<TUser, TRole>(this IServiceCollection services, Action<IdentityOptions>? setupAction = null)
        where TUser : ApplicationUser
        where TRole : IdentityRole
    {
        IdentityBuilder builder;
        if (setupAction == null)
            builder = services.AddIdentity<TUser, TRole>();
        else
            builder = services.AddIdentity<TUser, TRole>(setupAction);

        builder.AddUserManager<ApplicationUserManager<TUser>>()
            .AddSignInManager<ApplicationUserSignInManager<TUser>>()
            .AddClaimsPrincipalFactory<ApplicationUserClaimsPrincipalFactory<TUser>>()
            .AddUserValidator<PhoneNumberValidator<TUser>>()
            .AddErrorDescriber<ApplicationUserIdentityErrorDescriber>();

        // 移除原有的PasswordValidator
        var passwordValidatorDescriptor = services.FirstOrDefault(d => d.ServiceType == typeof(IPasswordValidator<TUser>));
        if (passwordValidatorDescriptor != null)
        {
            services.Remove(passwordValidatorDescriptor);
        }
        // 添加AlphaIdPasswordValidator
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
