if($args.Count -eq 0)
{
    "请至少输入一个参数"
    exit
}

$args[0]

dotnet ef migrations add $args[0] -c IDSubjectsDbContext -o Migrations/IDSubjectsDb
dotnet ef migrations add $args[0] -c DirectoryLogonDbContext -o Migrations/DirectoryLogonDb
dotnet ef migrations add $args[0] -c ConfigurationDbContext -o Migrations/ConfigurationDb
dotnet ef migrations add $args[0] -c OperationalDbContext -o Migrations/OperationalDb
