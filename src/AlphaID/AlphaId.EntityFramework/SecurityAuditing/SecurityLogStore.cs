using IdSubjects.SecurityAuditing;

namespace AlphaId.EntityFramework.SecurityAuditing;
internal class SecurityLogStore : IQueryableAuditLogStore
{
    private readonly LoggingDbContext dbContext;

    public SecurityLogStore(LoggingDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public IQueryable<AuditLogEntry> Log => this.dbContext.AuditLog;
}
