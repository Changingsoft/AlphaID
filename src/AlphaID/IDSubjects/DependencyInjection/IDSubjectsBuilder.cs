using IdSubjects.Diagnostics;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace IdSubjects.DependencyInjection;

/// <summary>
/// IdSubjects builder for DI.
/// </summary>
public class IdSubjectsBuilder
{
    /// <summary>
    /// Gets the service collection.
    /// </summary>
    public IServiceCollection Services { get; }

    /// <summary>
    /// 获取 AspNetCore Identity 基础设施提供的 IdentityBuilder.
    /// </summary>
    public IdentityBuilder IdentityBuilder { get; }

    /// <summary>
    /// Create new builder using incoming service collection.
    /// </summary>
    /// <param name="services"></param>
    /// <param name="identityBuilder"></param>
    public IdSubjectsBuilder(IServiceCollection services, IdentityBuilder identityBuilder)
    {
        this.Services = services;
        this.IdentityBuilder = identityBuilder;
    }

    /// <summary>
    /// 添加自然人管理拦截器。
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public IdSubjectsBuilder AddNaturalPersonManagerInterceptor<T>() where T : class, IInterceptor
    {
        this.Services.AddScoped<IInterceptor, T>();
        return this;
    }

    /// <summary>
    /// Add natural person store implementation into the system.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public IdSubjectsBuilder AddPersonStore<T>() where T : IUserStore<NaturalPerson>
    {
        this.Services.TryAddScoped(typeof(IUserStore<NaturalPerson>), typeof(T));
        return this;
    }

    /// <summary>
    /// Add generic organization store implementation into the system.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public IdSubjectsBuilder AddOrganizationStore<T>() where T : IOrganizationStore
    {
        this.Services.TryAddScoped(typeof(IOrganizationStore), typeof(T));
        return this;
    }

    /// <summary>
    /// Add organization member store implementation into the system.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public IdSubjectsBuilder AddOrganizationMemberStore<T>() where T : IOrganizationMemberStore
    {
        this.Services.TryAddScoped(typeof(IOrganizationMemberStore), typeof(T));
        return this;
    }
}
