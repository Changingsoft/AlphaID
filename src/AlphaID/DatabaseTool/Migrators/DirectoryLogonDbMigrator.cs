using AlphaID.DirectoryLogon.EntityFramework;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace DatabaseTool.Migrators;
internal class DirectoryLogonDbMigrator : DatabaseMigrator
{
    private readonly DirectoryLogonDbContext db;

    public DirectoryLogonDbMigrator(DirectoryLogonDbContext db)
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
        var directoryLogonDataSqlFiles = Directory.GetFiles("./TestingData/DirectoryLogonData", "*.sql");
        foreach (var file in directoryLogonDataSqlFiles)
        {
            await this.db.Database.ExecuteSqlRawAsync(await File.ReadAllTextAsync(file, Encoding.UTF8));
        }
    }
}
