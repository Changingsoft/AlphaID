using Duende.IdentityServer.EntityFramework.DbContexts;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace DatabaseTool.Migrators;
internal class IdServerConfigurationDbMigrator : DatabaseMigrator
{
    private readonly ConfigurationDbContext db;

    public IdServerConfigurationDbMigrator(ConfigurationDbContext db)
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
        var configDbSqlFiles = Directory.GetFiles("./TestingData/ConfigurationDbContext", "*.sql");
        foreach (var file in configDbSqlFiles)
        {
            await this.db.Database.ExecuteSqlRawAsync(await File.ReadAllTextAsync(file, Encoding.UTF8));
        }
    }
}
