using IdSubjects.SecurityAuditing;

namespace AlphaId.EntityFramework.SecurityAuditing;

internal class SecurityLogStore(LoggingDbContext dbContext) : IQueryableAuditLogStore
{
    public IQueryable<AuditLogEntry> Log => dbContext.AuditLog;
}