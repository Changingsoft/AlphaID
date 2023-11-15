using AlphaId.RealName.EntityFramework;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace DatabaseTool.Migrators;
internal class RealNameDbMigrator : DatabaseMigrator
{
    readonly RealNameDbContext db;

    public RealNameDbMigrator(RealNameDbContext db)
    {
        this.db = db;
    }

    public override async Task AddTestingDataAsync()
    {
        var idSubjectsDbSqlFiles = Directory.GetFiles("./TestingData/RealNameDbContext", "*.sql");
        foreach (var file in idSubjectsDbSqlFiles)
        {
            await this.db.Database.ExecuteSqlRawAsync(await File.ReadAllTextAsync(file, Encoding.UTF8));
        }
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
}
