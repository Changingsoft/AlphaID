using Microsoft.Extensions.Options;

namespace DatabaseTool;

/// <summary>
/// 表示数据库执行器
/// </summary>
internal class DatabaseExecutor(IEnumerable<DatabaseMigrator> migrators, IOptions<DatabaseExecutorOptions> options, ILogger<DatabaseExecutor>? logger)
{
    private readonly DatabaseExecutorOptions _options = options.Value;

    public IEnumerable<DatabaseMigrator> Migrators { get; } = migrators;

    public async Task ExecuteAsync()
    {


        //Step1: DropDatabase
        logger?.LogDebug("正在准备执行第1阶段（删除数据库）");
        if (_options.DropDatabase)
        {
            foreach (var migrator in Migrators)
                await migrator.DropDatabaseAsync();
        }

        //Step2: Migrate
        logger?.LogDebug("正在准备执行第2阶段（建立/迁移数据库）");
        if (_options.ApplyMigrations)
        {
            foreach (var migrator in Migrators)
                await migrator.MigrateAsync();
        }

        //Step3: PostMigrations
        logger?.LogDebug("正在准备执行第3阶段（迁移后处理）");
        if (_options.ApplyMigrations)
        {
            foreach (var migrator in Migrators)
                await migrator.PostMigrationAsync();
        }

        //Step4: AddTestingData
        logger?.LogDebug("正在准备执行第4阶段（准备测试数据）");
        if (_options.AddTestingData)
        {
            foreach (var migrator in Migrators)
                await migrator.AddTestingDataAsync();
        }
    }
}