using System.Text;
using AlphaId.DirectoryLogon.EntityFramework;
using Microsoft.EntityFrameworkCore;

namespace DatabaseTool.Migrators;

internal class DirectoryLogonDbMigrator(DirectoryLogonDbContext db) : DatabaseMigrator(db)
{
    public override async Task AddTestingDataAsync()
    {
        string[] directoryLogonDataSqlFiles = Directory.GetFiles("./TestingData/DirectoryLogonData", "*.sql");
        foreach (string file in directoryLogonDataSqlFiles)
            await db.Database.ExecuteSqlRawAsync(await File.ReadAllTextAsync(file, Encoding.UTF8));
    }
}