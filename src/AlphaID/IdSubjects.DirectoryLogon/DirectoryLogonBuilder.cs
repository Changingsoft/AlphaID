using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace IdSubjects.DirectoryLogon;

/// <summary>
/// 目录管理构造器。
/// </summary>
/// <remarks>
/// </remarks>
/// <param name="services"></param>
public class DirectoryLogonBuilder(IServiceCollection services)
{
    /// <summary>
    /// </summary>
    public IServiceCollection Services { get; } = services;

    /// <summary>
    /// 向基础结构添加目录服务存取器。
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public DirectoryLogonBuilder AddDirectoryServiceStore<T>() where T : class, IDirectoryServiceDescriptorStore
    {
        Services.TryAddScoped<IDirectoryServiceDescriptorStore, T>();
        return this;
    }

    /// <summary>
    /// 向基础结构添加登录账号存取器。
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public DirectoryLogonBuilder AddLogonAccountStore<T>() where T : class, IDirectoryAccountStore
    {
        Services.TryAddScoped<IDirectoryAccountStore, T>();
        return this;
    }
}