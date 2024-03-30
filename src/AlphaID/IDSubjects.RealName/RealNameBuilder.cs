using IdSubjects.RealName.Requesting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace IdSubjects.RealName;
/// <summary>
/// 实名认证服务构建器。
/// </summary>
/// <remarks>
/// 初始化实名认证服务构建器。
/// </remarks>
/// <param name="services"></param>
public class RealNameBuilder(IServiceCollection services)
{

    /// <summary>
    /// 获取服务集合。
    /// </summary>
    public IServiceCollection Services { get; } = services;

    /// <summary>
    /// 添加实名认证存储。
    /// </summary>
    /// <typeparam name="T">一个实现<see cref="IRealNameAuthenticationStore"/>的可实例化类型。</typeparam>
    /// <returns></returns>
    public RealNameBuilder AddRealNameAuthenticationStore<T>() where T : class, IRealNameAuthenticationStore
    {
        Services.TryAddScoped<IRealNameAuthenticationStore, T>();
        return this;
    }

    /// <summary>
    /// 添加实名请求存储。
    /// </summary>
    /// <typeparam name="T">一个实现<see cref="IRealNameRequestStore"/>的可实例化类型。</typeparam>
    /// <returns></returns>
    public RealNameBuilder AddRealNameRequestStore<T>() where T : class, IRealNameRequestStore
    {
        Services.TryAddScoped<IRealNameRequestStore, T>();
        return this;
    }
}
