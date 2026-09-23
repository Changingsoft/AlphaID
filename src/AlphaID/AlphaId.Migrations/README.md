# AlphaID Migrations

Alpha ID 数据库迁移资料。

## 1 迁移前准备

安装EFCore工具：

```
dotnet tool install --global dotnet-ef
```

项目必须引用`Microsoft.EntityFrameworkCore.Design`nuget包。使用以下命令可以添加此包。

```
dotnet add package Microsoft.EntityFrameworkCore.Design
```

## 2 创建迁移

包括多个DbContext，在创建迁移时，需使用参数 `--context` 指定DbContext，需使用 `--output-dir` 指定迁移代码的输出位置，以方便管理。
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

 `Add-Migrations.ps1` 保存了同一份 `DbContext` → 输出目录 映射，可一次性生成全部 8 条迁移，无需逐条输入：

``` powershell
.\Add-Migrations.ps1 <Migration Title>
.\Add-Migrations.ps1 <Migration Title> -Framework net8.0
```

要查看某个 `DbContext` 已应用的迁移，可用 `migrations list`（注意同样需要 `--project` / `--startup-project` / `--framework` 三个参数）：

``` powershell
dotnet ef migrations list --context AlphaIdDbContext --project ../AlphaId.Migrations/AlphaId.Migrations.csproj --startup-project DatabaseTool.csproj --framework net10.0 --no-build
```
