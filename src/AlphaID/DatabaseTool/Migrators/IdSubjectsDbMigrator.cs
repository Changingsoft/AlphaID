using System.Text;
using AlphaId.EntityFramework;
using Microsoft.EntityFrameworkCore;

namespace DatabaseTool.Migrators;

internal class IdSubjectsDbMigrator(IdSubjectsDbContext db) : DatabaseMigrator(db)
{
    public override async Task AddTestingDataAsync()
    {
        string[] idSubjectsDbSqlFiles = Directory.GetFiles("./TestingData/IDSubjectsDbContext", "*.sql");
        foreach (string file in idSubjectsDbSqlFiles)
            await db.Database.ExecuteSqlRawAsync(await File.ReadAllTextAsync(file, Encoding.UTF8));
    }
}