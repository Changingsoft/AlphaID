using IdSubjects;
using Microsoft.AspNetCore.Identity;

#pragma warning disable IDE0130 // 命名空间与文件夹结构不匹配
// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection;
#pragma warning restore IDE0130 // 命名空间与文件夹结构不匹配

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
}
