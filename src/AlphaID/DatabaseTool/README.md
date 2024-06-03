# Database Tool

Alpha ID ���ݿ�Ǩ�ƹ��ߡ����ڳ�ʼ�����ݿ⣬����Ǩ�����ݿ⣬��Ӳ������ݡ�

## �÷�

**����������ִ�����ݿ����ǰ�������ȷ�����ݿ��ѱ��ݡ�**

ִ�й���ǰ��������appsettings.json����ȷ�������ݿ������ַ�����

Ȼ���������л�Powershell��ִ��Ǩ�ƹ��ߣ�

``` powershell
.\DatabaseTool.exe [DropDatabase=<false:true>] [AddTestingData=<false:true>]
```

### ��������

`DropDatabase = <false:true>` ָʾ�Ƿ�ɾ�����ݿ⡣Ĭ��Ϊfalse��

**�˿��ؽ�ɾ�����ݿ⣬�����С��ʹ�ô˿��ء���ִ�в���ǰ������ر������ݿ�����ݡ�**

`AddTestingData = <false:true>` ָʾ�Ƿ���Ӳ������ݡ�Ĭ��Ϊfalse���˿�����Ҫ�����ڿ����������Ի���Ԥ����������ʱʹ�á�

���ɲ�����Ҫʹ�øò������ݣ���ִ�м��ɲ���ǰ��Ӧʹ�ô˿������������ݡ�

### ����

������ϵͳִ�����ݿ�Ǩ��������Ǩ������ͨ�����ᶪʧ���ݣ������ܻ������ݿ�ܹ����������������̬�����仯��ִ�д˲���ǰ����ر������ݿ�����ݣ��Ա�Ǩ��ʧ�ܺ���ˡ�

```
.\DatabaseTool.exe
```

��ʼ��ȫ�µ����ݿ⣬�����ڵ�һ�β���

```
.\DatabaseTool.exe DropDatabase=true
```

## �����ͱ�д

### ����ԭ��

���ݿ�Ǩ�Ʒ�Ϊ�ĸ��׶Σ�

- DropDatabase�׶Σ��˽׶ν�ɾ�����ݿ⡣
- ApplyMigrations�׶Σ��˽׶ΰ�˳��Ӧ��Ǩ�ƣ������γ����䵱ǰ�汾�����ݿ�ܹ���
- PostMigrations�׶Σ��˽׶���Ǩ�ƺ�����
- AddTestingData�׶Σ��˽׶����ڲ����ʺϿ������ԵĲ������ݡ�

ӦΪÿ��DbContext��дǨ����DatabaseMigrator���������д���ÿ���׶ε��ض�����
 
��ʼ�������󣬴��� DatabaseExecutor������ DatabaseExecutorOptions �����ã����׶�˳��ֽ׶ε���Ǩ����������������ݿ�����ݵĳ�ʼ��������

### ����ǰ׼��

����Ҫ�Ȱ�װEFCore���ߣ�

```
dotnet tool install --global dotnet-ef
```

ʹ������������Ը��¹��߰汾��

```
dotnet tool update --global dotnet-ef
```

Ҫʹ�øù��ߣ���Ŀ��������`Microsoft.EntityFrameworkCore.Design`nuget����ʹ���������������Ӵ˰���

```
dotnet add package Microsoft.EntityFrameworkCore.Design
```

### ����Ǩ��

����Ŀ�������DbContext���ڴ���Ǩ��ʱ����ʹ�ò���`-c`ָ��DbContext����ʹ��`-o`�����ö�Ǩ�ƴ�������λ�ã��Է������

����Ǩ�Ƶ���ϸ�������£�

``` powershell
dotnet ef migrations add <Migration Title> -c OperationalDbContext -o Migrations/AdminWebAppDb
dotnet ef migrations add <Migration Title> -c ConfigurationDbContext -o Migrations/ConfigurationDb
dotnet ef migrations add <Migration Title> -c DirectoryLogonDbContext -o Migrations/DirectoryLogonDb
dotnet ef migrations add <Migration Title> -c IDSubjectsDbContext -o Migrations/IDSubjectsDb
dotnet ef migrations add <Migration Title> -c LoggingDbContext -o Migrations/LoggingDb
dotnet ef migrations add <Migration Title> -c PersistedGrantDbContext -o Migrations/PersistedGrantDb
dotnet ef migrations add <Migration Title> -c RealNameDbContext -o Migrations/RealNameDb
```

### Ǩ���ڼ��ʼ������

�������ݿ���ʹ��`migrationBuilder.Sql(string)`��������SQL��䷽ʽִ�����ݳ�ʼ�����������ϴ����Ҫ������������ʱ������ʹ��SQL�ű��ļ�����ʼ�����ݣ���������Ϊ���������Ŀ¼�С�

���Ѵ�����Ǩ�����Up�׶Σ���ȡSQL�ű���Ӧ�á�

���Ǩ��ǰ����Ҫ��������״̬ת����������Up�׶κ�Down�׶ηֱ��������ͷ���ת����

�˴����������Ǩ�ƽ׶ζ�ȡSQL�ű���ִ�С�

``` c#
protected override void Up(MigrationBuilder migrationBuilder)
{
    // �ļ�·��������ڳ�����ľ���·��
    migrationBuilder.Sql(File.ReadAllText("Migrations/ConfigurationDb/<FileName>.sql"));
}
```