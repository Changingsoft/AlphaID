using System.Text;
using Duende.IdentityServer.EntityFramework.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace DatabaseTool.Migrators;

internal class IdServerConfigurationDbMigrator(ConfigurationDbContext db) : DatabaseMigrator(db)
{
    public override async Task AddTestingDataAsync()
    {
        string[] configDbSqlFiles = Directory.GetFiles("./TestingData/ConfigurationDbContext", "*.sql");
        foreach (string file in configDbSqlFiles)
            await db.Database.ExecuteSqlRawAsync(await File.ReadAllTextAsync(file, Encoding.UTF8));
    }
}