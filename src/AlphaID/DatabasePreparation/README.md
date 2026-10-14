# DatabasePreparation

可复用的「数据库准备」抽象：把建库流程拆成**按 `DbContext` 装配的初始化单元**，
再由统一执行器按阶段顺序驱动它们。

## 边界：这里只有机制，没有业务

本工程**刻意保持零业务耦合**，以便整目录复制到其他解决方案、或作为独立 NuGet 包发布：

| 约束 | 含义 |
|---|---|
| **没有任何 `ProjectReference`** | 不认识任何业务工程，不知道有哪些 `DbContext` |
| **不引用数据库提供程序包** | 只有 `Microsoft.EntityFrameworkCore.Relational`（`EnsureDeleted`/`Migrate` 需要它）；SqlServer、Npgsql 等一律不管，具体是哪个库由调用方决定 |
| **数据经委托注入** | 「灌什么内置数据 / 样例数据」是调用方传进来的 lambda，业务概念不进来 |

换句话说：**机制住在这里（阶段顺序、删库与迁移、初始化单元的装配契约），
策略住在业务侧（有哪些表、灌什么数据）。**

AlphaID 的那一层策略在 `AlphaId.DatabasePreparation` 工程：

```
DatabasePreparation（通用抽象，零业务依赖）
        ▲
        │  ProjectReference
        │
AlphaId.DatabasePreparation（AlphaID 适配层：八个上下文 + 内置数据 + 样例数据）
        ▲
        ├── DatabaseTool
        └── IntegrationTestUtilities
```

**新增一个业务概念时请不要往本工程加** —— 那正是这里需要保持的边界。
把两层合并（让 `DatabasePreparation` 直接引用 `AlphaId.*`）会立刻毁掉它的可复用性。

## 1. 它解决什么问题

每个项目都要有一份「建库」逻辑：初始化数据库、升级 schema、补齐内置数据、（开发时）灌样例数据。
多数人于是各写一份，于是：

- 数据库工具与集成测试各写一套，**两边悄悄长偏**；
- 每个新项目都要重抄一遍，稍作改动就埋下顺序错误；
- 顺序（先删库、再迁移、再灌数据）散落各处，改一处就可能破坏另一处。

本工程把这件事拆成两部分：

```
建库 = 一组 IDatabaseInitializer  +  一个 DatabasePreparationExecutor
```

执行器只认「一组初始化单元」，不认识任何具体的 `DbContext`，
并保证**阶段之间存在全局屏障**：所有单元都做完第 1 阶段，再都做第 2 阶段。

## 2. 四个阶段

| # | 阶段 | 语义 | 默认 |
|---|---|---|---|
| 1 | 删库 | 不可逆 | 关 |
| 2 | 迁移 | 顺序前进一次 | **开** |
| 3 | 补齐内置数据 | **幂等**（缺失则插入、已存在则跳过） | **开** |
| 4 | 灌样例数据 | **非幂等**（要求库是干净的） | 关 |

顺序**只写在执行器一处**，调用方无法绕过：

1. 删库必须最先，否则残留的表结构会与迁移冲突；
2. 迁移必须早于任何数据写入，否则表还不存在；
3. 内置数据必须早于样例数据——样例数据往往引用内置数据里的记录，顺序颠倒会让样例数据指向不存在的实体。

## 3. 怎么用

### 3.1 最简情形：只删库 + 迁移

大多数上下文属于这一类，**不需要写任何实现类**：

```csharp
var initializers = new IDatabaseInitializer[]
{
    new DbContextInitializer<OrderDbContext>(orderDb),
    new DbContextInitializer<CatalogDbContext>(catalogDb),
};

var executor = new DatabasePreparationExecutor(initializers, logger);
await executor.ExecuteAsync(new DatabasePrepareOptions { DropDatabase = true });
```

`DbContextInitializer<T>` 的四个方法都有默认空实现，未提供委托的两步**什么都不做**。

### 3.2 有内置数据要补齐

传入一个委托即可（`TContext` 是强类型的，不需要强转）。
内置数据委托多收一个 `DatabasePrepareOptions`：像 `OverwriteInitData` 这类写入策略
是**本次执行**的开关，不该在装配时就被固化。

```csharp
new DbContextInitializer<OrderDbContext>(
    orderDb,
    seedInitData: async (db, options, ct) =>
    {
        await db.SavedCards.CountAsync(ct);                 // 幂等示例
        // db.Add(new Country { Code = "CN", Name = "中国" });
        // await db.SaveChangesAsync(ct);
    });
```

### 3.3 有样例数据要灌

样例数据通常住在**另一个项目**（测试专用），因此同样用委托接入，
而不是让抽象层引用它：

```csharp
new DbContextInitializer<OrderDbContext>(
    orderDb,
    seedTestingData: (db, ct) => SampleDataSeeder.SeedOrdersAsync(db, ct));
```

**边界很重要**：抽象层不认识「样例数据」住在哪。把它接进来是调用方的决定，
于是样例数据不会顺着抽象进入生产程序集（`AlphaId/` 子目录就是那层「调用方的决定」）。

若开了 `AddTestingData` 却**没有任何**单元声明该职责，执行器会在动手**之前**抛异常——
而不是静默地什么也不写，让问题在很远的地方以断言失败的形式冒出来。
单元是否声明职责由 `HandlesTestingData` 表达，`DbContextInitializer<T>` 依据
「是否真的传了 `seedTestingData`」自动填写。

### 3.4 多个上下文指向不同的库

执行器不关心上下文之间是什么关系：连同一个库（重复删除安全）或各连各的服务器都可以。
需要控制顺序时，**装配顺序即执行顺序**：

```csharp
// 需要「客户端」先于「订单」建库时，把客户端的单元排在前面。
IDatabaseInitializer[] initializers = [clientInitializer, orderInitializer];
```

### 3.5 接入 DI

```csharp
services.AddDbContext<OrderDbContext>(o => o.UseSqlServer(connectionString));
services.AddDbContext<CatalogDbContext>(o => o.UseSqlServer(connectionString));

services.AddScoped<IDatabaseInitializer>(sp =>
    new DbContextInitializer<OrderDbContext>(sp.GetRequiredService<OrderDbContext>()));
services.AddScoped<IDatabaseInitializer>(sp =>
    new DbContextInitializer<CatalogDbContext>(sp.GetRequiredService<CatalogDbContext>()));

services.AddScoped<DatabasePreparationExecutor>();
```

> 前提：迁移与 `DbContext` 同处一个程序集。若分属不同程序集，
> 每个 `UseSqlServer` 都要传 `sql => sql.MigrationsAssembly("...")`，
> **漏写不会报错，只会让 `Migrate()` 建出一个空库**——把迁移和 `DbContext` 放一起可从根上避免。

## 4. AlphaID 侧怎么用的

本工程对此一无所知——那正是设计意图。AlphaID 的装配在 **`AlphaId.DatabasePreparation`** 工程，
它把八个 `DbContext`（六个自有上下文 + IdentityServer 的配置库与授权库）、内置数据与样例数据
接到执行器上：

```csharp
services.AddAlphaIdDatabasePreparation(connectionString);
// ...
DatabasePreparationExecutor executor = provider.GetRequiredService<DatabasePreparationExecutor>();
await executor.ExecuteAsync(options);
```

`DatabaseTool` 与集成测试的 `TestDatabase` 走的**就是这一个入口**：

| | DatabaseTool | 集成测试 |
|---|---|---|
| 装配 | `AddAlphaIdDatabasePreparation(连接串)` | 同左 |
| 执行器 | `DatabasePreparationExecutor` | 同左 |
| 差异 | `DropDatabase` 取自配置 | `DropDatabase=false`，删库改用 `SINGLE_USER` 强制方式 |
| 差异 | `AddTestingData` 取自配置（生产为 false） | 恒为 `true` |

因此「工具建的库」与「测试建的库」在 schema 与数据上必然一致——
这正是把逻辑收到一处的原因：过去两者各写一套，工具多灌了什么、测试少跑了一步都不会有提示。

## 5. 迁移与 `DbContext` 的位置

迁移 ID 来自 `[Migration("…")]` 特性，**与命名空间、文件名、工程位置都无关**。
因此调整文件位置或命名空间不会影响已迁移的数据库；反之改动特性值会让已迁移的库
认为某个迁移尚未应用，从而重复执行。

但**迁移与 `DbContext` 是否同处一个程序集，很要紧**：分属两个程序集时，
每个 `UseSqlServer` 都得传 `sql => sql.MigrationsAssembly("...")`，
而**漏写不会报错，只会让 `Migrate()` 建出一个空库**。
AlphaID 因此把两者放进了 `AlphaId.EntityFramework`（内置数据也在那里），
Duende 的两个上下文统一经 `AlphaIdSqlServerOptionsExtensions.UseAlphaIdSqlServer(...)` 指定。

样例数据住在 `AlphaId.TestingData`，只被 `AlphaId.DatabasePreparation` 引用，
而产品程序集不引用那一层，因此它不会进入生产。
