using AlphaId.DirectoryLogon.EntityFramework;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace DatabaseTool.Migrators;
internal class DirectoryLogonDbMigrator(DirectoryLogonDbContext db) : DatabaseMigrator
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
        var directoryLogonDataSqlFiles = Directory.GetFiles("./TestingData/DirectoryLogonData", "*.sql");
        foreach (var file in directoryLogonDataSqlFiles)
        {
            await db.Database.ExecuteSqlRawAsync(await File.ReadAllTextAsync(file, Encoding.UTF8));
        }
    }
}
