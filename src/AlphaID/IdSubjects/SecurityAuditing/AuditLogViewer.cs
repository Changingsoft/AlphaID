namespace IdSubjects.SecurityAuditing;

/// <summary>
/// 审计日志查看器。
/// </summary>
/// <remarks>
/// </remarks>
/// <param name="store"></param>
public class AuditLogViewer(IQueryableAuditLogStore store)
{
    /// <summary>
    /// 获取按时间戳倒序的审计日志。
    /// </summary>
    public IQueryable<AuditLogEntry> Log => store.Log.OrderByDescending(l => l.TimeStamp);
}