using IdSubjects.SecurityAuditing;
using Microsoft.EntityFrameworkCore;

namespace AlphaId.EntityFramework.SecurityAuditing;
public class LoggingDbContext : DbContext
{
    public LoggingDbContext(DbContextOptions<LoggingDbContext> options) : base(options)
    {
    }

    public DbSet<AuditLogEntry> AuditLog { get; protected set; } = default!;
}
