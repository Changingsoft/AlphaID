namespace IdSubjects.SecurityAuditing;

/// <summary>
/// 可查询审计日志存取器接口。
/// </summary>
public interface IQueryableAuditLogStore
{
    /// <summary>
    /// 获取可查询审计日志。
    /// </summary>
    IQueryable<AuditLogEntry> Log { get; }
}