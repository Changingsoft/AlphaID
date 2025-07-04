using Microsoft.EntityFrameworkCore;
using System.Text;
using AlphaId.EntityFramework;

namespace DatabaseTool.Migrators;

internal class AlphaIdDbMigrator(AlphaIdDbContext db) : DatabaseMigrator(db)
{
    public override async Task AddTestingDataAsync()
    {
        string[] files = Directory.GetFiles("./TestingData/AlphaIdData", "*.sql");
        foreach (string file in files)
            await db.Database.ExecuteSqlRawAsync(await File.ReadAllTextAsync(file, Encoding.UTF8));
    }
}