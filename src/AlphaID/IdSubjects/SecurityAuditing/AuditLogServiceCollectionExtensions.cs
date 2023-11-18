using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace IdSubjects.SecurityAuditing;

/// <summary>
/// 审计日志服务的DI注册扩展。
/// </summary>
public static class AuditLogServiceCollectionExtensions
{
    /// <summary>
    /// 向基础DI添加审计日志服务。
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static AuditLogBuilder AddAuditLog(this IServiceCollection services)
    {
        services.TryAddScoped<AuditLogViewer>();

        return new AuditLogBuilder(services);
    }
}