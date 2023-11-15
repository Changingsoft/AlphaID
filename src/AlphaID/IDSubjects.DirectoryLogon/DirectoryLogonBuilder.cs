using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace IdSubjects.DirectoryLogon;

/// <summary>
/// 目录管理构造器。
/// </summary>
public class DirectoryLogonBuilder
{
    /// <summary>
    /// 
    /// </summary>
    public IServiceCollection Services { get; }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="services"></param>
    public DirectoryLogonBuilder(IServiceCollection services)
    {
        this.Services = services;
    }

    /// <summary>
    /// 向基础结构添加目录服务存取器。
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public DirectoryLogonBuilder AddDirectoryServiceStore<T>() where T : class, IDirectoryServiceStore
    {
        this.Services.TryAddScoped<IDirectoryServiceStore, T>();
        return this;
    }

    /// <summary>
    /// 向基础结构添加登录账号存取器。
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public DirectoryLogonBuilder AddLogonAccountStore<T>() where T : class, ILogonAccountStore
    {
        this.Services.TryAddScoped<ILogonAccountStore, T>();
        return this;
    }
}
