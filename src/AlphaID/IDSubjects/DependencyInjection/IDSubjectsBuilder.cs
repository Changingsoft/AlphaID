using IdSubjects.Diagnostics;
using IdSubjects.Invitations;
using IdSubjects.Payments;
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
    /// Gets the service collection.
    /// </summary>
    public IServiceCollection Services { get; }

    /// <summary>
    /// 获取 AspNetCore Identity 基础设施提供的 IdentityBuilder.
    /// </summary>
    public IdentityBuilder IdentityBuilder { get; }

    /// <summary>
    /// 添加自然人管理拦截器。
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public IdSubjectsBuilder AddInterceptor<T>() where T : class, IInterceptor
    {
        this.Services.AddScoped<IInterceptor, T>();
        return this;
    }

    /// <summary>
    /// Add natural person store implementation into the system.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public IdSubjectsBuilder AddPersonStore<T>() where T : class, INaturalPersonStore
    {
        this.Services.TryAddScoped<INaturalPersonStore, T>();
        this.IdentityBuilder.AddUserStore<T>(); //As IUserStore<NaturalPerson>
        return this;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public IdSubjectsBuilder AddPasswordHistoryStore<T>() where T : class, IPasswordHistoryStore
    {
        this.Services.TryAddScoped<IPasswordHistoryStore, T>();
        return this;
    }

    /// <summary>
    /// Add generic organization store implementation into the system.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public IdSubjectsBuilder AddOrganizationStore<T>() where T : class, IOrganizationStore
    {
        this.Services.TryAddScoped<IOrganizationStore, T>();
        return this;
    }

    /// <summary>
    /// Add organization member store implementation into the system.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public IdSubjectsBuilder AddOrganizationMemberStore<T>() where T : class, IOrganizationMemberStore
    {
        this.Services.TryAddScoped<IOrganizationMemberStore, T>();
        return this;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public IdSubjectsBuilder AddPersonBankAccountStore<T>() where T : class, IPersonBankAccountStore
    {
        this.Services.TryAddScoped<IPersonBankAccountStore, T>();
        return this;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public IdSubjectsBuilder AddJoinOrganizationInvitationStore<T>() where T : class, IJoinOrganizationInvitationStore
    {
        this.Services.TryAddScoped<IJoinOrganizationInvitationStore, T>();
        return this;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public IdSubjectsBuilder AddOrganizationBankAccountStore<T>() where T : class, IOrganizationBankAccountStore
    {
        this.Services.TryAddScoped<IOrganizationBankAccountStore, T>();
        return this;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public IdSubjectsBuilder AddOrganizationIdentifierStore<T>() where T : class, IOrganizationIdentifierStore
    {
        this.Services.TryAddScoped<IOrganizationIdentifierStore, T>();
        return this;
    }
}
