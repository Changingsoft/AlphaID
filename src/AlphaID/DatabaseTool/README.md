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

> **配置节**：`DatabasePreparationOptions`。它直接绑定 `DatabasePreparation.DatabasePrepareOptions`，
> 与集成测试用的是**同一个类型**，因此不再存在「工具与测试各有一套开关」的可能。
> 旧配置节 `DatabaseExecutorOptions` 仍会被读取，同一键上新节优先，既有部署脚本不必立刻改。
>
> 旧节里的 `ExecutePostMigrations` 已被移除：从来没有任何迁移器实现过迁移后处理，
> 它在本次改动前是个死开关，现在这个阶段整体不存在了。

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

**本工具不含任何建库逻辑。** 它只做三件事：读配置 → 打印并确认 → 调用执行器。
真正的逻辑分两层：通用抽象（阶段顺序、删库与迁移）在 `DatabasePreparation` —— 该层**零业务依赖**，
可整目录复制到其他解决方案；AlphaID 的装配（有哪些上下文、灌什么数据）在 `AlphaId.DatabasePreparation`。
集成测试与本工具走**完全同一份实现**。

数据库准备分为四个阶段，由 `DatabasePreparation.DatabasePreparationExecutor` 按固定顺序驱动：

| # | 阶段 | 语义 | 默认 |
|---|---|---|---|
| 1 | `DropDatabase` | 不可逆 | 关 |
| 2 | `ApplyMigrations` | 顺序前进一次 | **开** |
| 3 | `AddInitData` | **幂等**（缺失则插入、已存在则跳过） | **开** |
| 4 | `AddTestingData` | **非幂等**（要求库是干净的） | 关 |

顺序**只写在执行器一处**，调用方无法绕过：

1. 删库必须最先，否则残留的表结构会与迁移冲突；
2. 迁移必须早于任何数据写入，否则表还不存在；
3. 内置数据必须早于样例数据——样例数据往往引用内置数据里的记录。

装配在 `AlphaId.DatabasePreparation/AlphaIdDatabasePreparation.cs`：它注册八个 `DbContext`
（六个自有上下文 + IdentityServer 的配置库与授权库）以及对应的 `IDatabaseInitializer`。
新增一个 `DbContext` 只需在那里加一个初始化单元，工具与集成测试**同时**获得它，不必改动任何一方。

### 2.1.1 内置数据的位置

内置数据定义在 `AlphaId.EntityFramework/InitData` 目录下（与 `ConfigurationDbContext` 同居一个工程）：
`InitData.*.cs` 以 C# 常量描述数据（客户端、API 范围、标识资源），`InitDataSeeder.EnsureAsync`
负责幂等地把数据写入数据库。它由 `AlphaIdDatabasePreparation` 在 `AddInitData` 阶段调用：

```csharp
services.AddScoped<IDatabaseInitializer>(sp => new DbContextInitializer<ConfigurationDbContext>(
    sp.GetRequiredService<ConfigurationDbContext>(),
    seedInitData: (db, options, ct) => EnsureInitDataAsync(db, options, sp.GetService<ILoggerFactory>(), ct),
    name: "IdentityServer配置库"));
```

`InitDataSeeder.EnsureAsync` 整体幂等；也可以按表分别调用 `EnsureApiScopesAsync` / `EnsureIdentityResourcesAsync` / `EnsureClientsAsync`。
它返回的 `InitDataResult` 带有 `Inserted`、`Skipped` 与 `Warnings`，调用方据此记录日志。

`InitDataEnsureOptions.OverwriteExisting` 直接取自 `DatabasePrepareOptions.OverwriteInitData`——
**这是把开关传下去的方式**：阶段选项在执行时才确定，不应在装配时固化。

客户端的 `ClientSecret` 不存明文：代码中保留明文常量，写库前经 `InitDataSeeder.HashSecret` 转成 `base64(SHA256(明文))`，
与 ASP.NET Core 的密钥散列约定一致。

请不要再新增 T-SQL 脚本来存放内置数据：脚本依赖运行时当前目录、只有 SQL Server 能执行，
且以 `INSERT` 写入的脚本无法重复执行。数据是 C# 常量后，集成测试可以自行补齐，不再依赖 `DatabaseTool`。

修改 `InitData` 中的数值时请注意：这些数据与历史 SQL 脚本灌出的数据库是逐字段一致的（包括 `Created` / `Updated` 时间戳、
字段里的标点与大小写），不要为了「好看」而改动它们。改动后请重新运行 `DatabaseTool`，差异会以告警形式给出。

### 2.1.2 测试数据的位置

测试数据定义在 `AlphaId.TestingData` 项目中：`SampleData.*.cs` 描述数据（自然人、组织、角色分配），
`SampleDataSeeder.SeedAsync` 负责把数据写入数据库。它由 `AlphaIdDatabasePreparation` 在 `AddTestingData` 阶段接入：

```csharp
services.AddScoped<IDatabaseInitializer>(sp => new DbContextInitializer<AlphaIdIdentityDbContext>(
    sp.GetRequiredService<AlphaIdIdentityDbContext>(),
    seedTestingData: (db, ct) => SampleDataSeeder.SeedAsync(db, cancellationToken: ct),
    name: "身份库"));
```

`SampleDataSeeder.SeedAsync` 有按 `DbContext` 区分的三个重载，集成测试也可以只传入需要的那几条数据
（例如 `SampleDataSeeder.SeedAsync(db, [SampleData.LiuBei])`）。

**该工程仅供开发调试与集成测试使用**：它只被 `AlphaId.DatabasePreparation` 引用，而那一层
不被任何产品程序集（WebApp / WebAPI）引用，因此样例数据不会进入生产。

请不要再新增 T-SQL 脚本来存放测试数据：脚本依赖运行时当前目录、只有 SQL Server 能执行，
且二进制字段必须以十六进制文本内嵌。

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

### 2.3 创建迁移

> **迁移与 `DbContext` 同住 `AlphaId.EntityFramework` 工程**（`Migrations/` 子目录），
> 内置数据也在同一工程（`InitData/`）。这样 EF Core 无需 `MigrationsAssembly(...)` 即可找到迁移。
> 此前迁移住在独立工程，每一处 `UseSqlServer` 都得显式指定迁移程序集，
> 而**漏写不会报错，只会让 `Migrate()` 建出一个空库**——这是最难查的一类故障。

本工程包括多个 DbContext，在创建迁移时，需使用参数 `--context` 指定 DbContext，需使用 `--output-dir` 指定迁移代码的输出位置。
由于 `AlphaId.EntityFramework` 多目标（`net8.0;net10.0`），`dotnet ef` 还必须显式指定 `--framework`；
并用 `--startup-project` 指向 `DatabaseTool`——类库不能作为启动工程，由本工具提供各 `DbContext` 的装配与连接字符串。

`AlphaId.EntityFramework/Add-Migrations.ps1` 保存了同一份 `DbContext` → 输出目录 映射，可一次性生成全部 8 条迁移，无需逐条输入：

``` powershell
.\Add-Migrations.ps1 <Migration Title>
.\Add-Migrations.ps1 <Migration Title> -Framework net8.0
```

单条生成（以 `net10.0` 为例）：

``` powershell
dotnet ef migrations add <Migration Title> --context AlphaIdDbContext --project ../AlphaId.EntityFramework/AlphaId.EntityFramework.csproj --startup-project DatabaseTool.csproj --framework net10.0 --output-dir Migrations/AlphaIdDb
```

要查看某个 `DbContext` 已应用的迁移，可用 `migrations list`（注意同样需要 `--project` / `--startup-project` / `--framework` 三个参数）：

``` powershell
dotnet ef migrations list --context AlphaIdDbContext --project ../AlphaId.EntityFramework/AlphaId.EntityFramework.csproj --startup-project DatabaseTool.csproj --framework net10.0 --no-build
```

> 迁移 ID 来自 `[Migration("…")]` 特性，**与命名空间、文件名、工程位置都无关**。
> 因此把迁移从 `AlphaId.Migrations` 搬到 `AlphaId.EntityFramework` 不会影响已迁移的数据库。
> 反之，改动特性里的值会让已迁移的库认为某个迁移尚未应用，从而重复执行。

### 2.4 数据初始化

内置数据与测试数据都不通过迁移来写入，而是在迁移完成之后由 `AddInitData` / `AddTestingData` 阶段补齐：

- 内置数据 → `AlphaId.EntityFramework/InitData`（见 2.1.1）
- 测试数据 → `AlphaId.TestingData`（见 2.1.2）

两者都应由 `AlphaIdDatabasePreparation` 注册的初始化单元调用，不要写进 `Migrations` 目录里的 `migrationBuilder.Sql`：

- 迁移一旦生成就被视为「已发布」，后续修改不会在已迁移的库上再次执行，而内置数据是需要被反复校正的；
- 迁移里的 `INSERT` 无法重复执行（主键冲突），而这两个阶段都要求幂等；
- 迁移是「按顺序只能前进一次」的，而数据补齐需要「每次运行都核对一遍」。

只有在**迁移前后需要进行数据状态转换**（把旧架构下的既有数据改写成新架构的形状）时，才应使用
`migrationBuilder.Sql`，并且必须在 Up 阶段和 Down 阶段分别进行正向和反向转换。

### 2.5 连接数据库的代码写在哪

**不要直接调用 `UseSqlServer`**，一律用 `AlphaId.EntityFramework` 里的 `UseAlphaIdSqlServer(...)`：

```csharp
options.UseAlphaIdSqlServer(connectionString);
```

它统一指定迁移程序集与 NTS 拓扑扩展。八个上下文必须给出完全一致的配置——
少一处的 `MigrationsAssembly` 或 `UseNetTopologySuite`，那个上下文就会静默地迁移出与别人不同的 schema。
