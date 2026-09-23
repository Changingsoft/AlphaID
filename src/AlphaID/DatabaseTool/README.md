# Database Tool

Alpha ID 数据库迁移工具。用于初始化数据库，升级迁移数据库，添加内置数据和测试数据。

## 1. 用法

**<font color="red">在生产环境执行数据库操作前，请务必确认数据库已备份。</font>**

1. 执行工具前，请先在`appsettings.Production.json`中正确配置数据库连接字符串。

> 强烈建议不要直接修改`appsettings.json`的内容，如果没有找到`appsettings.Production.json`，可以在`appsettings.json`所在的位置创建一个新的。

2. 然后在命令行或Powershell中执行迁移工具：

``` powershell
.\DatabaseTool.exe [DropDatabase=<false:true>] [AddInitData=<false:true>] [OverwriteInitData=<false:true>] [AddTestingData=<false:true>]
```

### 1.1 更改环境

默认情况下以`Production`环境执行，若要更改环境，可以添加`--environment`参数，支持的参数包括`Development|Production`，例如：

```powershell
.\DatabaseTool.exe --environment Development
```

### 1.2 参数开关

`DropDatabase = <false:true>` 指示是否删除数据库。默认为false。

**此开关将删除数据库，请务必小心使用此开关。在执行操作前，请务必备份数据库和数据。**

`AddInitData = <false:true>` 指示是否补齐内置数据。默认为true。

内置数据是系统运行所必需的（IdentityServer 的客户端、API 范围、标识资源），详见 [`docs/InitData.md`](../../../docs/InitData.md)。
此阶段的写入是**幂等**的：以自然键查找，缺失则插入、已存在则跳过，因此重复执行不会报错。**一般情况下不要关闭此开关。**

`OverwriteInitData = <false:true>` 指示当数据库中已存在的内置数据与代码不一致时，是否用代码中的定义覆盖它。默认为false。

默认行为是「跳过 + 差异告警」：已经存在但与代码不一致的行会输出告警，但不会被修改。
只有在确认要用代码中的定义覆盖数据库现有值时才应打开此开关。**请先备份数据库。**

`AddTestingData = <false:true>` 指示是否添加测试数据。默认为false。此开关主要用于在开发环境调试或在预览环境评估时使用。

集成测试既不依赖内置数据也不依赖测试数据预先存在：它会自己建库、自行应用迁移，并自行补齐这两类数据，
无需在执行集成测试前运行本工具。详见 [`docs/Development.md`](../../../docs/Development.md) 的「集成测试」一节。

> `ExecutePostMigrations` 开关在本次改动前未被实际读取（第3阶段的判断条件误用了 `ApplyMigrations`），因此当时该开关不生效；现已修正。`Program.cs` 会把各开关的最终取值打印出来，可作为确认依据。

### 1.3 样例

向已有系统执行数据库迁移升级。迁移升级通常不会丢失数据，但可能会因数据库架构变更而导致数据形态发生变化。执行此操作前请务必备份数据库，以便迁移失败后回退。

```
.\DatabaseTool.exe
```

初始化全新的数据库，适用于第一次部署

```
.\DatabaseTool.exe DropDatabase=true
```

## 2. 开发和编写

### 2.1 工作原理

数据库迁移分为五个阶段：

- DropDatabase阶段，此阶段将删除数据库。
- ApplyMigrations阶段，此阶段按顺序应用迁移，最终形成适配当前版本的数据库架构。
- PostMigrations阶段，此阶段在迁移后运行
- AddInitData阶段，此阶段用于补齐系统运行所必需的内置数据。
- AddTestingData阶段，此阶段用于插入适合开发调试的测试数据。

应为每个DbContext编写迁移器DatabaseMigrator，并在其中处理每个阶段的特定任务。迁移代码本身位于类库项目 `AlphaId.Migrations`（见 2.3）。

初始化环境后，创建 DatabaseExecutor，根据 DatabaseExecutorOptions 的设置，按阶段顺序分阶段调用迁移器。最终完成数据库和数据的初始化工作。

### 2.1.1 内置数据的位置

内置数据定义在 `AlphaId.InitData` 项目中：`InitData.*.cs` 以 C# 常量描述数据（客户端、API 范围、标识资源），
`InitDataSeeder.EnsureAsync` 负责幂等地把数据写入数据库。迁移器只需在自己的 `AddInitDataAsync` 中调用它：

```csharp
public override async Task AddInitDataAsync()
{
    InitDataResult result = await InitDataSeeder.EnsureAsync(db, new InitDataEnsureOptions
    {
        OverwriteExisting = options.Value.OverwriteInitData,
    });
    foreach (string warning in result.Warnings)
        logger.LogWarning("{Warning}", warning);
}
```

`InitDataSeeder.EnsureAsync` 整体幂等；也可以按表分别调用 `EnsureApiScopesAsync` / `EnsureIdentityResourcesAsync` / `EnsureClientsAsync`。
它返回的 `InitDataResult` 带有 `Inserted`、`Skipped` 与 `Warnings`，调用方据此记录日志。

客户端的 `ClientSecret` 不存明文：代码中保留明文常量，写库前经 `InitDataSeeder.HashSecret` 转成 `base64(SHA256(明文))`，
与 ASP.NET Core 的密钥散列约定一致。

请不要再往 `InitData` 目录里新增 T-SQL 脚本（该目录已随本次改动删除）：脚本依赖运行时当前目录、只有 SQL Server 能执行，
且以 `INSERT` 写入的脚本无法重复执行。数据是 C# 常量后，集成测试可以自行补齐，不再依赖 `DatabaseTool`。

修改 `InitData` 中的数值时请注意：这些数据与历史 SQL 脚本灌出的数据库是逐字段一致的（包括 `Created` / `Updated` 时间戳、
字段里的标点与大小写），不要为了「好看」而改动它们。改动后请重新运行 `DatabaseTool`，差异会以告警形式给出。

### 2.1.2 测试数据的位置

测试数据本身定义在 `AlphaId.TestingData` 项目中：`SampleData.*.cs` 描述数据（自然人、组织、角色分配），
`SampleDataSeeder.SeedAsync` 负责把数据写入数据库。迁移器只需在自己的 `AddTestingDataAsync` 中调用对应的重载：

```csharp
public override Task AddTestingDataAsync()
{
    return SampleDataSeeder.SeedAsync(db);
}
```

`SampleDataSeeder.SeedAsync` 有按 `DbContext` 区分的三个重载，集成测试也可以只传入需要的那几条数据
（例如 `SampleDataSeeder.SeedAsync(db, [SampleData.LiuBei])`）。

请不要再往 `TestingData` 目录里新增 T-SQL 脚本：脚本依赖运行时当前目录、只有 SQL Server 能执行，
且二进制字段必须以十六进制文本内嵌。数据是 C# 常量后，集成测试可以自行灌库，不再依赖 `DatabaseTool`。

修改 `SampleData` 中的数值时请注意：这些数据与历史 SQL 脚本灌出的数据库是逐字节一致的，
连经纬度的小数位都被刻意保留，不要为了「好看」而取整。

### 2.2 开发前准备

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

### 2.3 创建迁移

> **迁移代码位于类库项目 `AlphaId.Migrations`，不在 DatabaseTool 中。** 这样做是为了让集成测试能够自行建库，
> 而不必引用一个命令行工具。请不要再把迁移写回 DatabaseTool：`MigrationsAssembly` 已改为引用该项目的程序集名
> （`AlphaId.Migrations.MigrationAssembly.Name`），用字符串字面量写错位置时 `Migrate()` 只会建出空库且不报错。

本项目包括多个DbContext，在创建迁移时，需使用参数 `--context` 指定DbContext，需使用 `--output-dir` 指定迁移代码的输出位置，以方便管理。
由于 `AlphaId.Migrations` 多目标（`net8.0;net10.0`），`dotnet ef` 还必须显式指定 `--framework`；
并用 `--project` / `--startup-project` 分别指向迁移工程与启动工程（`--startup-project` 仍需是 DatabaseTool，由它提供各 `DbContext` 的装配）。

创建迁移的详细命令如下（以 `net10.0` 为例）：

``` powershell
dotnet ef migrations add <Migration Title> --context OperationalDbContext --project ../AlphaId.Migrations/AlphaId.Migrations.csproj --startup-project DatabaseTool.csproj --framework net10.0 --output-dir Migrations/AdminWebAppDb
dotnet ef migrations add <Migration Title> --context ConfigurationDbContext --project ../AlphaId.Migrations/AlphaId.Migrations.csproj --startup-project DatabaseTool.csproj --framework net10.0 --output-dir Migrations/ConfigurationDb
dotnet ef migrations add <Migration Title> --context DirectoryLogonDbContext --project ../AlphaId.Migrations/AlphaId.Migrations.csproj --startup-project DatabaseTool.csproj --framework net10.0 --output-dir Migrations/DirectoryLogonDb
dotnet ef migrations add <Migration Title> --context AlphaIdIdentityDbContext --project ../AlphaId.Migrations/AlphaId.Migrations.csproj --startup-project DatabaseTool.csproj --framework net10.0 --output-dir Migrations/AlphaIdIdentityDb
dotnet ef migrations add <Migration Title> --context LoggingDbContext --project ../AlphaId.Migrations/AlphaId.Migrations.csproj --startup-project DatabaseTool.csproj --framework net10.0 --output-dir Migrations/LoggingDb
dotnet ef migrations add <Migration Title> --context PersistedGrantDbContext --project ../AlphaId.Migrations/AlphaId.Migrations.csproj --startup-project DatabaseTool.csproj --framework net10.0 --output-dir Migrations/PersistedGrantDb
dotnet ef migrations add <Migration Title> --context RealNameDbContext --project ../AlphaId.Migrations/AlphaId.Migrations.csproj --startup-project DatabaseTool.csproj --framework net10.0 --output-dir Migrations/RealNameDb
dotnet ef migrations add <Migration Title> --context AlphaIdDbContext --project ../AlphaId.Migrations/AlphaId.Migrations.csproj --startup-project DatabaseTool.csproj --framework net10.0 --output-dir Migrations/AlphaIdDb
```

本目录提供的 `Add-Migrations.ps1` 保存了同一份 `DbContext` → 输出目录 映射，可一次性生成全部 8 条迁移，无需逐条输入：

``` powershell
.\Add-Migrations.ps1 <Migration Title>
.\Add-Migrations.ps1 <Migration Title> -Framework net8.0
```

要查看某个 `DbContext` 已应用的迁移，可用 `migrations list`（注意同样需要 `--project` / `--startup-project` / `--framework` 三个参数）：

``` powershell
dotnet ef migrations list --context AlphaIdDbContext --project ../AlphaId.Migrations/AlphaId.Migrations.csproj --startup-project DatabaseTool.csproj --framework net10.0 --no-build
```

### 2.4 数据初始化

内置数据与测试数据都不通过迁移来写入，而是在迁移完成之后由 `AddInitData` / `AddTestingData` 阶段补齐：

- 内置数据 → `AlphaId.InitData`（见 2.1.1）
- 测试数据 → `AlphaId.TestingData`（见 2.1.2）

两者都应由迁移器调用，不要写进 `Migrations` 目录里的 `migrationBuilder.Sql`：

- 迁移一旦生成就被视为「已发布」，后续修改不会在已迁移的库上再次执行，而内置数据是需要被反复校正的；
- 迁移里的 `INSERT` 无法重复执行（主键冲突），而这两个阶段都要求幂等；
- 迁移是「按顺序只能前进一次」的，而数据补齐需要「每次运行都核对一遍」。

只有在**迁移前后需要进行数据状态转换**（把旧架构下的既有数据改写成新架构的形状）时，才应使用
`migrationBuilder.Sql`，并且必须在Up阶段和Down阶段分别进行正向和反向转换。
