using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace IdSubjects.SecurityAuditing;

/// <summary>
/// 审计日志构建器。
/// </summary>
/// <remarks>
/// 
/// </remarks>
/// <param name="services"></param>
public class AuditLogBuilder(IServiceCollection services)
{

    /// <summary>
    /// 
    /// </summary>
    public IServiceCollection Services { get; } = services;

    /// <summary>
    /// 添加审计日志存取器。
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public AuditLogBuilder AddLogStore<T>() where T : class, IQueryableAuditLogStore
    {
        Services.TryAddScoped<IQueryableAuditLogStore, T>();
        return this;
    }
}