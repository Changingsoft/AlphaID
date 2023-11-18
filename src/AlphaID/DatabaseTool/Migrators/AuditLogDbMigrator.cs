using AlphaId.EntityFramework.SecurityAuditing;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseTool.Migrators;
internal class AuditLogDbMigrator:DatabaseMigrator
{
    private readonly LoggingDbContext dbContext;

    public AuditLogDbMigrator(LoggingDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public override async Task DropDatabaseAsync()
    {
        await this.dbContext.Database.EnsureDeletedAsync();
    }

    public override async Task MigrateAsync()
    {
        await this.dbContext.Database.MigrateAsync();
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
