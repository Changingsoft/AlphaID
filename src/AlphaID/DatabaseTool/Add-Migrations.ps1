if($args.Count -eq 0)
{
    "请至少输入一个参数，第一个参数用于指定迁移名称。"
    exit
}

$args[0]
dotnet build

dotnet ef migrations add $args[0] -c IDSubjectsDbContext -o Migrations/IDSubjectsDb --no-build
dotnet ef migrations add $args[0] -c DirectoryLogonDbContext -o Migrations/DirectoryLogonDb --no-build
dotnet ef migrations add $args[0] -c ConfigurationDbContext -o Migrations/ConfigurationDb --no-build
dotnet ef migrations add $args[0] -c PersistedGrantDbContext -o Migrations/PersistedGrantDb --no-build