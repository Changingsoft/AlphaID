using AdminWebApp.Infrastructure.DataStores;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace DatabaseTool.Migrators;
internal class AdminCenterDbMigrator(OperationalDbContext db) : DatabaseMigrator
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
        var adminWebAppTestingFiles = Directory.GetFiles("./TestingData/AdminWebAppData", "*.sql");
        foreach (var file in adminWebAppTestingFiles)
        {
            await db.Database.ExecuteSqlRawAsync(await File.ReadAllTextAsync(file, Encoding.UTF8));
        }
    }
}
