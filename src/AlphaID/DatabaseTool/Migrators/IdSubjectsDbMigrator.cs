using AlphaId.EntityFramework;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace DatabaseTool.Migrators;
internal class IdSubjectsDbMigrator(IdSubjectsDbContext db) : DatabaseMigrator
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
        var idSubjectsDbSqlFiles = Directory.GetFiles("./TestingData/IDSubjectsDbContext", "*.sql");
        foreach (var file in idSubjectsDbSqlFiles)
        {
            await db.Database.ExecuteSqlRawAsync(await File.ReadAllTextAsync(file, Encoding.UTF8));
        }
    }
}
