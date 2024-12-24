namespace DatabaseTool;

internal class DatabaseExecutorOptions
{
    /// <summary>
    /// </summary>
    public bool DropDatabase { get; set; } = false;

    public bool ApplyMigrations { get; set; } = true;

    public bool ExecutePostMigrations { get; set; } = true;

    public bool AddTestingData { get; set; } = false;
}