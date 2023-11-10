using Duende.IdentityServer.EntityFramework.DbContexts;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace DatabaseTool.Migrators;
internal class IdServerPersistedGrantDbMigrator : DatabaseMigrator
{
    private readonly PersistedGrantDbContext db;

    public IdServerPersistedGrantDbMigrator(PersistedGrantDbContext db)
    {
        this.db = db;
    }

    public override async Task DropDatabaseAsync()
    {
        await this.db.Database.EnsureDeletedAsync();
    }

    public override async Task MigrateAsync()
    {
        await this.db.Database.MigrateAsync();
    }

    public override Task PostMigrationAsync()
    {
        return Task.CompletedTask;
    }

    public override async Task AddTestingDataAsync()
    {
        var idserverOperationalSqlFiles = Directory.GetFiles("./TestingData/PersistedGrantDbContext", "*.sql");
        foreach (var file in idserverOperationalSqlFiles)
        {
            await this.db.Database.ExecuteSqlRawAsync(await File.ReadAllTextAsync(file, Encoding.UTF8));
        }
    }
}
