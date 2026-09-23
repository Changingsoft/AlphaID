using AlphaId.EntityFramework;
using AlphaId.TestingData;

namespace DatabaseTool.Migrators;

internal class AlphaIdDbMigrator(AlphaIdDbContext db) : DatabaseMigrator(db)
{
    public override Task AddTestingDataAsync()
    {
        return SampleDataSeeder.SeedAsync(db);
    }
}
