using System.Text;
using Duende.IdentityServer.EntityFramework.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace DatabaseTool.Migrators;

internal class IdServerPersistedGrantDbMigrator(PersistedGrantDbContext db) : DatabaseMigrator(db)
{
    public override async Task AddTestingDataAsync()
    {
        string[] sqlFiles = Directory.GetFiles("./TestingData/PersistedGrantDbContext", "*.sql");
        foreach (string file in sqlFiles)
            await db.Database.ExecuteSqlRawAsync(await File.ReadAllTextAsync(file, Encoding.UTF8));
    }
}