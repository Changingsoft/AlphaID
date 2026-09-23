using AlphaId.EntityFramework.Admin;
using AlphaId.TestingData;

namespace DatabaseTool.Migrators;

internal class AdminCenterDbMigrator(OperationalDbContext db) : DatabaseMigrator(db)
{
    public override Task AddTestingDataAsync()
    {
        return SampleDataSeeder.SeedAsync(db);
    }
}