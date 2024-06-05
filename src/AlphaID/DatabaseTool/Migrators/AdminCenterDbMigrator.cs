using System.Text;
using AdminWebApp.Infrastructure.DataStores;
using Microsoft.EntityFrameworkCore;

namespace DatabaseTool.Migrators;

internal class AdminCenterDbMigrator(OperationalDbContext db) : DatabaseMigrator(db)
{
    public override async Task AddTestingDataAsync()
    {
        string[] adminWebAppTestingFiles = Directory.GetFiles("./TestingData/AdminWebAppData", "*.sql");
        foreach (string file in adminWebAppTestingFiles)
            await db.Database.ExecuteSqlRawAsync(await File.ReadAllTextAsync(file, Encoding.UTF8));
    }
}