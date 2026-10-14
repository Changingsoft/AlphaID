# DatabasePreparation

可复用的「数据库准备」抽象：把建库流程拆成**按 `DbContext` 装配的初始化单元**，
再由统一执行器按阶段顺序驱动它们。

本工程**不依赖任何业务工程**（没有 `ProjectReference`），也不引用任何数据库提供程序包——
删库与迁移只用 EF Core 的 `DatabaseFacade`，具体是 SQL Server 还是别的由调用方决定；
「灌什么内置数据 / 样例数据」通过委托从外部注入。因此整个目录可以直接复制到其他解决方案。

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
3. 内置数据必须早于样例数据——样例数据往往引用内置数据里的记录
   （例如短信任务引用的客户端 Id），顺序颠倒会让样例数据指向不存在的实体。

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

传入一个委托即可（`TContext` 是强类型的，不需要强转）：

```csharp
new DbContextInitializer<OrderDbContext>(
    orderDb,
    seedInitData: async (db, ct) =>
    {
        await db.SavedCards.CountAsync(ct);                 // 幂等示例
        // db.Add(new Country { Code = "CN", Name = "中国" });
        // await db.SaveChangesAsync(ct);
    });
```

### 3.3 有样例数据要灌

样例数据通常住在**另一个项目**（测试专用），因此同样用委托接入，
而不是让本工程引用它：

```csharp
new DbContextInitializer<OrderDbContext>(
    orderDb,
    seedTestingData: (db, ct) => SampleDataSeeder.SeedOrdersAsync(db, ct));
```

**边界很重要**：本工程不认识「样例数据」住在哪。把它接进来是调用方的决定，
于是样例数据不会顺着本工程进入生产程序集。

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

## 4. 迁移 ID 与命名空间

迁移 ID 来自 `[Migration("…")]` 特性，**与命名空间、文件名都无关**。
因此调整文件位置或命名空间不会影响已迁移的数据库；反之改动特性值会让已迁移的库
认为某个迁移尚未应用，从而重复执行。

## 5. PushCenter 侧怎么用的

推送中心用它装配两个上下文，唯一的接缝在
`ShortMessageServiceEF/PushCenterDatabaseInitializers.cs`（含「短信客户端登记」这类业务概念）；
`DatabaseTool` 与集成测试的 `TestDatabase` 装配**同一批**单元、交给**同一个**执行器，
差异只有 `DatabasePrepareOptions` 的取值。详见仓库 `docs/Arch.md`。
