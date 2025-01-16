using IdSubjects.SecurityAuditing;
using Microsoft.EntityFrameworkCore;

namespace AlphaId.EntityFramework.SecurityAuditing;

public class LoggingDbContext(DbContextOptions<LoggingDbContext> options) : DbContext(options)
{
    public DbSet<AuditLogEntry> AuditLog { get; protected set; } = null!;
}