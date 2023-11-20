namespace IdSubjects.SecurityAuditing;

/// <summary>
/// 审计日志查看器。
/// </summary>
public class AuditLogViewer
{
    private readonly IQueryableAuditLogStore store;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="store"></param>
    public AuditLogViewer(IQueryableAuditLogStore store)
    {
        this.store = store;
    }

    /// <summary>
    /// 获取按时间戳倒序的审计日志。
    /// </summary>
    public IQueryable<AuditLogEntry> Log => this.store.Log.OrderByDescending(l => l.TimeStamp);
}
