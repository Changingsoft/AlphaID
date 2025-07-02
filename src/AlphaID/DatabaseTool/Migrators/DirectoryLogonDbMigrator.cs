using AlphaId.EntityFramework.DirectoryAccountManagement;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace DatabaseTool.Migrators;

internal class DirectoryLogonDbMigrator(DirectoryLogonDbContext db) : DatabaseMigrator(db)
{
    public override async Task AddTestingDataAsync()
    {
        string[] files = Directory.GetFiles("./TestingData/DirectoryLogonData", "*.sql");
        foreach (string file in files)
            await db.Database.ExecuteSqlRawAsync(await File.ReadAllTextAsync(file, Encoding.UTF8));
    }
}