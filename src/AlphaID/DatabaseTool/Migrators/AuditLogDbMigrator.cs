using AlphaId.EntityFramework.SecurityAuditing;

namespace DatabaseTool.Migrators;

internal class AuditLogDbMigrator(LoggingDbContext dbContext) : DatabaseMigrator(dbContext);