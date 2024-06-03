# Database Tool

Alpha ID 数据库迁移工具。用于初始化数据库，升级迁移数据库，添加测试数据。

## 用法

**在生产环境执行数据库操作前，请务必确认数据库已备份。**

执行工具前，请先在appsettings.json中正确配置数据库连接字符串。

然后在命令行或Powershell中执行迁移工具：

``` powershell
.\DatabaseTool.exe [DropDatabase=<false:true>] [AddTestingData=<false:true>]
```

### 参数开关

`DropDatabase = <false:true>` 指示是否删除数据库。默认为false。

**此开关将删除数据库，请务必小心使用此开关。在执行操作前，请务必备份数据库和数据。**

`AddTestingData = <false:true>` 指示是否添加测试数据。默认为false。此开关主要用于在开发环境调试或在预览环境评估时使用。

集成测试需要使用该测试数据，在执行集成测试前，应使用此开关填充测试数据。

### 样例

向已有系统执行数据库迁移升级。迁移升级通常不会丢失数据，但可能会因数据库架构变更而导致数据形态发生变化。执行此操作前请务必备份数据库和数据，以便迁移失败后回退。

```
.\DatabaseTool.exe
```

初始化全新的数据库，适用于第一次部署

```
.\DatabaseTool.exe DropDatabase=true
```

## 开发和编写

### 工作原理

数据库迁移分为四个阶段：

- DropDatabase阶段，此阶段将删除数据库。
- ApplyMigrations阶段，此阶段按顺序应用迁移，最终形成适配当前版本的数据库架构。
- PostMigrations阶段，此阶段在迁移后运行
- AddTestingData阶段，此阶段用于插入适合开发调试的测试数据。

应为每个DbContext编写迁移器DatabaseMigrator，并在其中处理每个阶段的特定任务。
 
初始化环境后，创建 DatabaseExecutor，根据 DatabaseExecutorOptions 的设置，按阶段顺序分阶段调用迁移器。最终完成数据库和数据的初始化工作。

### 开发前准备

您需要先安装EFCore工具：

```
dotnet tool install --global dotnet-ef
```

使用下列命令可以更新工具版本：

```
dotnet tool update --global dotnet-ef
```

要使用该工具，项目必须引用`Microsoft.EntityFrameworkCore.Design`nuget包。使用以下命令可以添加此包。

```
dotnet add package Microsoft.EntityFrameworkCore.Design
```

### 创建迁移

本项目包括多个DbContext，在创建迁移时，需使用参数`-c`指定DbContext，需使用`-o`参数置顶迁移代码的输出位置，以方便管理。

创建迁移的详细命令如下：

``` powershell
dotnet ef migrations add <Migration Title> -c OperationalDbContext -o Migrations/AdminWebAppDb
dotnet ef migrations add <Migration Title> -c ConfigurationDbContext -o Migrations/ConfigurationDb
dotnet ef migrations add <Migration Title> -c DirectoryLogonDbContext -o Migrations/DirectoryLogonDb
dotnet ef migrations add <Migration Title> -c IDSubjectsDbContext -o Migrations/IDSubjectsDb
dotnet ef migrations add <Migration Title> -c LoggingDbContext -o Migrations/LoggingDb
dotnet ef migrations add <Migration Title> -c PersistedGrantDbContext -o Migrations/PersistedGrantDb
dotnet ef migrations add <Migration Title> -c RealNameDbContext -o Migrations/RealNameDb
```

### 迁移期间初始化数据

基础数据可以使用`migrationBuilder.Sql(string)`方法，以SQL语句方式执行数据初始化。数据量较大或需要大量批处理动作时，可以使用SQL脚本文件来初始化数据，将其设置为输出到生成目录中。

在已创建的迁移类的Up阶段，读取SQL脚本并应用。

如果迁移前后需要进行数据状态转换，必须在Up阶段和Down阶段分别进行正向和反向转换。

此处介绍如何在迁移阶段读取SQL脚本并执行。

``` c#
protected override void Up(MigrationBuilder migrationBuilder)
{
    // 文件路径必须基于程序根的绝对路径
    migrationBuilder.Sql(File.ReadAllText("Migrations/ConfigurationDb/<FileName>.sql"));
}
```