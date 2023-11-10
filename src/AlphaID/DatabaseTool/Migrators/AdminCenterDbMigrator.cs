using AdminWebApp.Infrastructure.DataStores;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace DatabaseTool.Migrators;
internal class AdminCenterDbMigrator : DatabaseMigrator
{
    private readonly OperationalDbContext db;

    public AdminCenterDbMigrator(OperationalDbContext db)
    {
        this.db = db;
    }

    public override Task DropDatabaseAsync()
    {
        return Task.CompletedTask;
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
        var adminWebAppTestingFiles = Directory.GetFiles("./TestingData/AdminWebAppData", "*.sql");
        foreach (var file in adminWebAppTestingFiles)
        {
            await this.db.Database.ExecuteSqlRawAsync(await File.ReadAllTextAsync(file, Encoding.UTF8));
        }
    }
}
