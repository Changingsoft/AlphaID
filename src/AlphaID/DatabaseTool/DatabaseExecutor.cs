using Microsoft.Extensions.Options;

namespace DatabaseTool;

/// <summary>
/// 表示数据库执行器
/// </summary>
internal class DatabaseExecutor
{
    private readonly DatabaseExecutorOptions options;
    private readonly ILogger<DatabaseExecutor>? logger;

    public DatabaseExecutor(IEnumerable<DatabaseMigrator> migrators, IOptions<DatabaseExecutorOptions> options, ILogger<DatabaseExecutor>? logger)
    {
        this.Migrators = migrators;
        this.logger = logger;
        this.options = options.Value;
    }

    public IEnumerable<DatabaseMigrator> Migrators { get; }

    public async Task ExecuteAsync()
    {


        //Step1: DropDatabase
        this.logger?.LogDebug("正在准备执行第1阶段（删除数据库）");
        if (this.options.DropDatabase)
        {
            foreach (var migrator in this.Migrators)
                await migrator.DropDatabaseAsync();
        }

        //Step2: Migrate
        this.logger?.LogDebug("正在准备执行第2阶段（建立/迁移数据库）");
        if (this.options.ApplyMigrations)
        {
            foreach (var migrator in this.Migrators)
                await migrator.MigrateAsync();
        }

        //Step3: PostMigrations
        this.logger?.LogDebug("正在准备执行第3阶段（迁移后处理）");
        if (this.options.ApplyMigrations)
        {
            foreach (var migrator in this.Migrators)
                await migrator.PostMigrationAsync();
        }

        //Step4: AddTestingData
        this.logger?.LogDebug("正在准备执行第4阶段（准备测试数据）");
        if (this.options.AddTestingData)
        {
            foreach (var migrator in this.Migrators)
                await migrator.AddTestingDataAsync();
        }
    }
}