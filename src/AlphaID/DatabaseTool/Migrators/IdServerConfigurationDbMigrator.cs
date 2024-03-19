using Duende.IdentityServer.EntityFramework.DbContexts;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace DatabaseTool.Migrators;
internal class IdServerConfigurationDbMigrator(ConfigurationDbContext db) : DatabaseMigrator
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
        var configDbSqlFiles = Directory.GetFiles("./TestingData/ConfigurationDbContext", "*.sql");
        foreach (var file in configDbSqlFiles)
        {
            await db.Database.ExecuteSqlRawAsync(await File.ReadAllTextAsync(file, Encoding.UTF8));
        }
    }
}
