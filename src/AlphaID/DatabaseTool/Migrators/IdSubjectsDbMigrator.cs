using AlphaId.EntityFramework.IdSubjects;
using AlphaId.TestingData;

namespace DatabaseTool.Migrators;

internal class IdSubjectsDbMigrator(AlphaIdIdentityDbContext db) : DatabaseMigrator(db)
{
    public override Task AddTestingDataAsync()
    {
        return SampleDataSeeder.SeedAsync(db);
    }
}