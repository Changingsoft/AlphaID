using Microsoft.EntityFrameworkCore;

namespace DatabaseTool;
/// <summary>
/// 表示一个数据库迁移器。
/// </summary>
internal abstract class DatabaseMigrator(DbContext dbContext)
{
    public virtual Task DropDatabaseAsync()
    {
        return dbContext.Database.EnsureDeletedAsync();
    }

    public virtual Task MigrateAsync()
    {
        return dbContext.Database.MigrateAsync();
    }

    public virtual Task PostMigrationAsync()
    {
        return Task.CompletedTask;
    }

    public virtual Task AddTestingDataAsync()
    {
        return Task.CompletedTask;
    }
}
