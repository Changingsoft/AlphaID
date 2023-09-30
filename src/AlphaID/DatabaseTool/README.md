# Database Tool

这是一个用来处理数据库迁移的工具。

它使用dotnet-ef工具处理数据库迁移脚本。

# 需求

您需要先安装EFCore工具：

```
dotnet tool install --global dotnet-ef
```

Update the tool using the following command:

```
dotnet tool update --global dotnet-ef
```

Before you can use the tools on a specific project, you'll need to add the Microsoft.EntityFrameworkCore.Design package to it.

```
dotnet add package Microsoft.EntityFrameworkCore.Design
```