# AlphaId.DatabasePreparation

`DatabasePreparation`（通用抽象）与 AlphaID 之间的**适配层**，也是唯一一个知道
「AlphaID 有哪些 `DbContext`、灌什么数据」的地方。

## 为什么要有这一层

`DatabasePreparation` 刻意保持零业务耦合——没有 `ProjectReference`、不引用任何数据库提供程序包——
以便整目录复制到其他解决方案或作为独立 NuGet 包发布。那条边界一旦破坏，它就退化成
「只能服务于 AlphaID 的代码」，也就没有剥离复用的价值了。

于是分工变成：

```
DatabasePreparation（通用抽象，零业务依赖）
  机制：阶段顺序、删库与迁移、初始化单元的装配契约
        ▲
        │ ProjectReference
        │
AlphaId.DatabasePreparation（本工程 = 策略）
  策略：八个 DbContext、内置数据、样例数据
        ▲
        ├── DatabaseTool
        └── IntegrationTestUtilities
```

**新增一个业务概念时请加在这里，不要加进 `DatabasePreparation`。**

## 它做什么

`AddAlphaIdDatabasePreparation(services, connectionString)` 一句话完成：

1. 装配六个自有上下文（经 `AddAlphaIdPlatform().AddEntityFramework(...)`）；
2. 装配 IdentityServer 的配置库与授权库；
3. 为八个上下文各注册一个 `IDatabaseInitializer`——
   `ConfigurationDbContext` 负责补齐内置数据，三个库负责灌样例数据，其余四个只参与删库与迁移；
4. 注册 `DatabasePreparationExecutor`。

`DatabaseTool` 与集成测试都只调用这一个入口，因此两者的建库流程不可能长偏。

## 与工具的差异

| | DatabaseTool | 集成测试 |
|---|---|---|
| 装配 | `AddAlphaIdDatabasePreparation(连接串)` | 同左 |
| 执行器 | `DatabasePreparationExecutor` | 同左 |
| `DropDatabase` | 取自配置 | `false`，删库改用 `SINGLE_USER` 强制方式 |
| `AddTestingData` | 取自配置（生产为 false） | 恒为 `true` |

## 新增一个 DbContext

只在 `AddAlphaIdDatabasePreparation` 里加一个初始化单元即可，工具与集成测试**同时**获得它：

```csharp
services.AddScoped<IDatabaseInitializer>(sp => new DbContextInitializer<NewDbContext>(
    sp.GetRequiredService<NewDbContext>(), name: "新库"));
```

若该库也要灌样例数据，再传 `seedTestingData:` 委托，并记得同步登记到
`ManagedContextTypes`——集成测试会逐个核对那里列出的上下文是否都已装配且可连接。

## 相关文件

| 位置 | 内容 |
|---|---|
| `AlphaId.EntityFramework/Migrations/` | 八个上下文的 EF 迁移（与 DbContext 同程序集） |
| `AlphaId.EntityFramework/InitData/` | IdentityServer 内置数据的定义与幂等写入 |
| `AlphaId.TestingData/` | 样例数据，仅开发调试与集成测试使用 |
| `AlphaId.EntityFramework/AlphaIdSqlServerOptionsExtensions.cs` | 八处 `UseSqlServer` 收敛到一处 |
