using Duende.IdentityServer.EntityFramework.DbContexts;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace DatabaseTool.Migrators;
internal class IdServerPersistedGrantDbMigrator(PersistedGrantDbContext db) : DatabaseMigrator
{
    public override async Task DropDatabaseAsync()
    {
        await db.Database.EnsureDeletedAsync();
    }

    public override Task MigrateAsync()
    {
        return db.Database.MigrateAsync();
    }

    public override Task PostMigrationAsync()
    {
        return Task.CompletedTask;
    }

    public override async Task AddTestingDataAsync()
    {
        var sqlFiles = Directory.GetFiles("./TestingData/PersistedGrantDbContext", "*.sql");
        foreach (var file in sqlFiles)
        {
            await db.Database.ExecuteSqlRawAsync(await File.ReadAllTextAsync(file, Encoding.UTF8));
        }
    }
}
