using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace IdSubjects.SecurityAuditing;

/// <summary>
/// 审计日志构建器。
/// </summary>
public class AuditLogBuilder
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="services"></param>
    public AuditLogBuilder(IServiceCollection services)
    {
        this.Services = services;
    }

    /// <summary>
    /// 
    /// </summary>
    public IServiceCollection Services { get; }

    /// <summary>
    /// 添加审计日志存取器。
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public AuditLogBuilder AddLogStore<T>() where T : class, IQueryableAuditLogStore
    {
        this.Services.TryAddScoped<IQueryableAuditLogStore, T>();
        return this;
    }
}