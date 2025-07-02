using AlphaId.EntityFramework.Admin;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace DatabaseTool.Migrators;

internal class AdminCenterDbMigrator(OperationalDbContext db) : DatabaseMigrator(db)
{
    public override async Task AddTestingDataAsync()
    {
        string[] files = Directory.GetFiles("./TestingData/AdminWebAppData", "*.sql");
        foreach (string file in files)
            await db.Database.ExecuteSqlRawAsync(await File.ReadAllTextAsync(file, Encoding.UTF8));
    }
}