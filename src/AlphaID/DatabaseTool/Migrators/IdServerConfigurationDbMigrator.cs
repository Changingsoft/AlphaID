using System.Text;
using Duende.IdentityServer.EntityFramework.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace DatabaseTool.Migrators;

internal class IdServerConfigurationDbMigrator(ConfigurationDbContext db) : DatabaseMigrator(db)
{
    public override async Task PostMigrationAsync()
    {
        string[] files = Directory.GetFiles("./InitData/IdentityServerConfiguration", "*.sql");
        foreach (string file in files)
            await db.Database.ExecuteSqlRawAsync(await File.ReadAllTextAsync(file, Encoding.UTF8));
    }

    public override async Task AddTestingDataAsync()
    {
        string[] files = Directory.GetFiles("./TestingData/ConfigurationDbContext", "*.sql");
        foreach (string file in files)
            await db.Database.ExecuteSqlRawAsync(await File.ReadAllTextAsync(file, Encoding.UTF8));
    }
}