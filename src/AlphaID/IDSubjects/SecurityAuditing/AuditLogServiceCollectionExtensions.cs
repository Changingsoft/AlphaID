using IdSubjects.SecurityAuditing;
using Microsoft.Extensions.DependencyInjection.Extensions;

#pragma warning disable IDE0130 // 命名空间与文件夹结构不匹配
namespace Microsoft.Extensions.DependencyInjection;
#pragma warning restore IDE0130 // 命名空间与文件夹结构不匹配

/// <summary>
///     审计日志服务的DI注册扩展。
/// </summary>
public static class AuditLogServiceCollectionExtensions
{
    /// <summary>
    ///     向基础DI添加审计日志服务。
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static AuditLogBuilder AddAuditLog(this IServiceCollection services)
    {
        services.TryAddScoped<AuditLogViewer>();

        return new AuditLogBuilder(services);
    }
}