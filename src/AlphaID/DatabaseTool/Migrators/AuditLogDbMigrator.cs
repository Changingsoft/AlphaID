using AlphaId.EntityFramework.SecurityAuditing;
using Microsoft.EntityFrameworkCore;

namespace DatabaseTool.Migrators;
internal class AuditLogDbMigrator(LoggingDbContext dbContext) : DatabaseMigrator
{
    public override async Task DropDatabaseAsync()
    {
        await dbContext.Database.EnsureDeletedAsync();
    }

    public override Task MigrateAsync()
    {
        return dbContext.Database.MigrateAsync();
    }

    public override Task PostMigrationAsync()
    {
        return Task.CompletedTask;
    }

    public override Task AddTestingDataAsync()
    {
        return Task.CompletedTask;
    }
}
