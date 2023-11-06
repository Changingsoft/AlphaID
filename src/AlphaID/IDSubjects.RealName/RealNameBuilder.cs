using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace IDSubjects.RealName;
/// <summary>
/// 
/// </summary>
public class RealNameBuilder
{
    private readonly IServiceCollection services;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="services"></param>
    public RealNameBuilder(IServiceCollection services)
    {
        this.services = services;
    }

    /// <summary>
    /// 添加实名认证相关存储。
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public RealNameBuilder AddRealNameStore<T>() where T : class, IRealNameStore
    {
        this.services.TryAddScoped<IRealNameStore, T>();
        return this;
    }
}
