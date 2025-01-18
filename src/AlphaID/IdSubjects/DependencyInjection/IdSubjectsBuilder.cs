using IdSubjects.Diagnostics;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace IdSubjects.DependencyInjection;

/// <summary>
///     IdSubjects builder for DI.
/// </summary>
/// <remarks>
///     Create new builder using incoming service collection.
/// </remarks>
/// <param name="services"></param>
/// <param name="identityBuilder"></param>
public class IdSubjectsBuilder(IServiceCollection services, IdentityBuilder identityBuilder)
{
    /// <summary>
    ///     Gets the service collection.
    /// </summary>
    public IServiceCollection Services { get; } = services;

    /// <summary>
    ///     获取 AspNetCore Identity 基础设施提供的 IdentityBuilder.
    /// </summary>
    public IdentityBuilder IdentityBuilder { get; } = identityBuilder;

    /// <summary>
    ///     添加自然人管理拦截器。
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public IdSubjectsBuilder AddInterceptor<T>() where T : class, IInterceptor
    {
        Services.AddScoped<IInterceptor, T>();
        return this;
    }

    /// <summary>
    ///     Add natural person store implementation into the system.
    /// </summary>
    /// <typeparam name="TStore"></typeparam>
    /// <typeparam name="TUser"></typeparam>
    /// <returns></returns>
    public IdSubjectsBuilder AddPersonStore<TStore, TUser>()
        where TStore : class, IApplicationUserStore<TUser>
        where TUser : ApplicationUser
    {
        Services.TryAddScoped<IApplicationUserStore<TUser>, TStore>();
        IdentityBuilder.AddUserStore<TStore>(); //As IUserStore<ApplicationUser>
        return this;
    }

    /// <summary>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public IdSubjectsBuilder AddPasswordHistoryStore<T>() where T : class, IPasswordHistoryStore
    {
        Services.TryAddScoped<IPasswordHistoryStore, T>();
        return this;
    }



    /// <summary>
    /// 使用指定的Profile生成器。
    /// </summary>
    /// <typeparam name="TGenerator"></typeparam>
    /// <typeparam name="TUser"></typeparam>
    /// <returns></returns>
    public IdSubjectsBuilder AddProfileUrlGenerator<TGenerator, TUser>() 
        where TGenerator : ProfileUrlGenerator<TUser>
        where TUser : ApplicationUser
    {
        Services.AddScoped<ProfileUrlGenerator<TUser>, TGenerator>();
        return this;
    }
}