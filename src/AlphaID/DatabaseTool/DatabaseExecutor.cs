using Microsoft.Extensions.Options;

namespace DatabaseTool;

/// <summary>
/// 表示数据库执行器
/// </summary>
internal class DatabaseExecutor
{
    private readonly DatabaseExecutorOptions options;

    public DatabaseExecutor(IEnumerable<DatabaseMigrator> migrators, IOptions<DatabaseExecutorOptions> options)
    {
        this.Migrators = migrators;
        this.options = options.Value;
    }

    public IEnumerable<DatabaseMigrator> Migrators { get; }

    public async Task ExecuteAsync()
    {
        foreach (var migrator in this.Migrators)
        {
            if (this.options.DropDatabase)
                await migrator.DropDatabaseAsync();
            if (this.options.ApplyMigrations)
                await migrator.MigrateAsync();
            if (this.options.ExecutePostMigrations)
                await migrator.PostMigrationAsync();
            if (this.options.AddTestingData)
                await migrator.AddTestingDataAsync();
        }
    }
}