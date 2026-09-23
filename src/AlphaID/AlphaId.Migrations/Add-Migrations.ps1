#Requires -Version 5.1
<#
.SYNOPSIS
    为每个 DbContext 生成一个新的 EF Core 迁移。

.DESCRIPTION
    迁移文件住在 AlphaId.Migrations 工程里，DatabaseTool 只作为「启动工程」提供各 DbContext 的装配与连接字符串。
    正因为两者分属不同工程，每条命令都必须显式给出 --project / --startup-project / --framework，
    否则 dotnet ef 无法判断该把迁移写到哪里、以及按哪个目标框架加载程序集。

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

$migrationsProject = Join-Path $PSScriptRoot "..\AlphaId.Migrations\AlphaId.Migrations.csproj"
$startupProject = Join-Path $PSScriptRoot "DatabaseTool.csproj"

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

Write-Output "迁移 $Name 已生成到 AlphaId.Migrations/Migrations 下。"
