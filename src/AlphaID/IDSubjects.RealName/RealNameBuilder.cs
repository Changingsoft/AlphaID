using IdSubjects.RealName.Requesting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace IdSubjects.RealName;
/// <summary>
/// 实名认证构造器。
/// </summary>
public class RealNameBuilder
{

    /// <summary>
    /// 
    /// </summary>
    /// <param name="services"></param>
    public RealNameBuilder(IServiceCollection services)
    {
        this.Services = services;
    }

    /// <summary>
    /// 
    /// </summary>
    public IServiceCollection Services { get; }

    /// <summary>
    /// 添加实名认证存储。
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public RealNameBuilder AddRealNameAuthenticationStore<T>() where T : class, IRealNameAuthenticationStore
    {
        this.Services.TryAddScoped<IRealNameAuthenticationStore, T>();
        return this;
    }

    /// <summary>
    /// 添加实名请求存储。
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public RealNameBuilder AddRealNameRequestStore<T>() where T : class, IRealNameRequestStore
    {
        this.Services.TryAddScoped<IRealNameRequestStore, T>();
        return this;
    }
}
