using AlphaId.EntityFramework.IdSubjects;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace DatabaseTool.Migrators;

internal class IdSubjectsDbMigrator(IdSubjectsDbContext db) : DatabaseMigrator(db)
{
    public override async Task AddTestingDataAsync()
    {
        string[] files = Directory.GetFiles("./TestingData/IdSubjectsDbContext", "*.sql");
        foreach (string file in files)
            await db.Database.ExecuteSqlRawAsync(await File.ReadAllTextAsync(file, Encoding.UTF8));
    }
}