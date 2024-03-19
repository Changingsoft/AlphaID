# Alpha ID 开发指南

## 开发环境要求

* [.NET](https://dotnet.microsoft.com/)
  * 本项目跟随 .NET LTS 策略，同时支持至少2个 LTS 版本，当前支持 6.0 和 8.0 。

> 可选要求
>
> 计划开发或研究示例程序时，您可能还需要安装下列环境组件：
>
> * Java Development Kit
> * Nodejs
>
> 如果您计划研究 LDAP 功能特性，则需要安装合适的 LDAP 服务，首选 AD LDS 组件。

集成开发环境（IDE）：

我们推荐使用 [Visual Studio 2022](https://visualstudio.microsoft.com/) ，推荐使用 [ReSharper](https://www.jetbrains.com/resharper/) 协助开发活动。

### 针对国际化和本地化的开发活动

为了更好提高国际化和本地化开发效率，可以额外安装 [ReSharper](https://www.jetbrains.com/resharper/) 或 [ResX Manager](https://marketplace.visualstudio.com/items?itemName=TomEnglert.ResXManager) 扩展。

## 调试

本地调试时，Alpha ID 不依赖实际的外部服务，它使用模拟的外部服务和本地持久化方案。

* 使用 [SQL Server Local DB](https://go.microsoft.com/fwlink/?LinkID=866658) 提供持久化。
* 不实际发送邮件，但会在日志中记录消息性日志。
* 不实际发送短信，但会在日志中记录消息性日志。
* 不实际发送短信验证码，但会在日志中记录消息性日志，对验证码的验证总是返回true。
* 不会实际执行OCR识别。

在调试阶段，你可以使用[示例数据](SampleData.md)来模拟完整的应用场景。

## 构建

在sln所在目录运行如下命令以进行构建：

``` powershell
dotnet build -c
```

你可以在 Visual Studio 中使用“生成解决方案”的方式构建项目。

## 测试

### 单元测试

我们使用 xUnit 来开展单元测试。开发阶段尽可能为关键代码和执行路径添加了单元测试。

如果您计划为此项目贡献代码，请确保为您的更改添加或调整单元测试，单元测试的设计应始终保持简单语义，并且您应保证完全通过单元测试。

### 集成测试

集成测试时，使用Development环境，与开发调试环境一致，使用[示例数据](SampleData.md)。

## 打包和发布

## 贡献

我们热忱期待你的贡献。如果你有意为该项目作贡献，请与作者联系。
