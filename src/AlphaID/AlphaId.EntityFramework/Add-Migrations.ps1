#Requires -Version 5.1
<#
.SYNOPSIS
    为每个 DbContext 生成一个新的 EF Core 迁移。

.DESCRIPTION
    迁移与 DbContext 同住 AlphaId.EntityFramework 工程，因此 --project 指向本工程自身。
    DatabaseTool 只作为「启动工程」，负责在设计时构造出八个上下文与连接字符串
    （真正的装配代码在 DatabasePreparation 里，与集成测试共用）。

    仍需显式给出 --project / --startup-project / --framework：
    - 本工程多目标（net8.0;net10.0），不指定 --framework 时 dotnet ef 无法判断按哪个 TFM 加载程序集；
    - 启动工程不是本工程（类库无法作为启动工程），必须指给 DatabaseTool；
    - 不指定 --output-dir 时 EF 会把迁移写进调用者所在目录，而不是本工程的 Migrations 下。

.EXAMPLE
    .\Add-Migrations.ps1 AddClientLogo

.EXAMPLE
    .\Add-Migrations.ps1 AddClientLogo -Framework net8.0
#>
param(
    [Parameter(Mandatory = $true, Position = 0)]
    [string]$Name,

    [string]$Framework = "net10.0"
)

[Console]::OutputEncoding = [System.Text.Encoding]::UTF8
[Console]::InputEncoding = [System.Text.Encoding]::UTF8

$migrationsProject = Join-Path $PSScriptRoot "AlphaId.EntityFramework.csproj"
$startupProject = Join-Path $PSScriptRoot "..\DatabaseTool\DatabaseTool.csproj"

Write-Output "迁移名称：$Name；目标框架：$Framework"

dotnet build $startupProject
if (-not $?) {
    Write-Error "构建失败，已中止。"
    exit 1
}

$contexts = @(
    @{ Context = "OperationalDbContext";      Folder = "AdminWebAppDb" },
    @{ Context = "ConfigurationDbContext";    Folder = "ConfigurationDb" },
    @{ Context = "DirectoryLogonDbContext";   Folder = "DirectoryLogonDb" },
    @{ Context = "AlphaIdIdentityDbContext";  Folder = "AlphaIdIdentityDb" },
    @{ Context = "LoggingDbContext";          Folder = "LoggingDb" },
    @{ Context = "PersistedGrantDbContext";   Folder = "PersistedGrantDb" },
    @{ Context = "RealNameDbContext";         Folder = "RealNameDb" },
    @{ Context = "AlphaIdDbContext";          Folder = "AlphaIdDb" }
)

foreach ($item in $contexts) {
    dotnet ef migrations add $Name `
        --context $item.Context `
        --project $migrationsProject `
        --startup-project $startupProject `
        --framework $Framework `
        --output-dir "Migrations/$($item.Folder)" `
        --no-build

    if (-not $?) {
        Write-Error "为 $($item.Context) 生成迁移失败，已中止。"
        exit 1
    }
}

Write-Output "迁移 $Name 已生成到 AlphaId.EntityFramework/Migrations 下。"
