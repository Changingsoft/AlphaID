using Duende.IdentityServer.EntityFramework.DbContexts;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace DatabaseTool.Migrators;

internal class IdServerPersistedGrantDbMigrator(PersistedGrantDbContext db) : DatabaseMigrator(db)
{
    public override async Task AddTestingDataAsync()
    {
        string[] files = Directory.GetFiles("./TestingData/PersistedGrantDbContext", "*.sql");
        foreach (string file in files)
            await db.Database.ExecuteSqlRawAsync(await File.ReadAllTextAsync(file, Encoding.UTF8));
    }
}