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
    /// 添加实名认证相关存储。
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public RealNameBuilder AddRealNameStore<T>() where T : class, IRealNameStateStore
    {
        this.Services.TryAddScoped<IRealNameStateStore, T>();
        return this;
    }
}
