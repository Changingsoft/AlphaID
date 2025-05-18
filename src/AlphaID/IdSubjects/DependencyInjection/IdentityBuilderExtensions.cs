using IdSubjects;
using Microsoft.AspNetCore.Identity;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Extensions for IdSubjects service injection.
/// </summary>
public static class IdentityBuilderExtensions
{
    /// <summary>
    /// 向IdentityBuilder添加一个自定义的ProfileUrlGenerator。
    /// </summary>
    /// <typeparam name="TGenerator"></typeparam>
    /// <typeparam name="TUser"></typeparam>
    /// <param name="builder"></param>
    /// <returns></returns>
    public static IdentityBuilder AddProfileUrlGenerator<TGenerator, TUser>(this IdentityBuilder builder)
        where TGenerator : ProfileUrlGenerator<TUser>
        where TUser : ApplicationUser
    {
        builder.Services.AddScoped<ProfileUrlGenerator<TUser>, TGenerator>();
        return builder;
    }

    /// <summary>
    /// 向IdentityBuilder添加一个自定义的PasswordHistoryStore。
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="builder"></param>
    /// <returns></returns>
    public static IdentityBuilder AddPasswordHistoryStore<T>(this IdentityBuilder builder)
        where T : class, IPasswordHistoryStore
    {
        builder.Services.AddScoped<IPasswordHistoryStore, T>();
        return builder;
    }
}
