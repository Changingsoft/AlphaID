# Alpha ID 开发指南

## 开发环境要求

* [.NET](https://dotnet.microsoft.com/)
  * 本项目跟随 .NET LTS 策略，当前支持 dotnet 8.0。

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

### 目录服务调试

在开发阶段，我们使用Active Directory Lightweight Directory Service（ADLDS，或称为AD轻型目录服务）来模拟目录服务。你需要安装ADLDS组件，使用向导创建ADLDS实例，用本库中的备份文件恢复实例状态，然后执行数据库脚本以添加目录服务。

详细信息请参阅[调试托管目录服务账户](/docs/DebugManagedAccount.md)

### 微信公众平台网页授权调试

AlphaID将在以下条件满足时触发微信公众平台网页授权：

* 已启用并正确配置了微信公众平台AppID和AppSecret
* 用户通过微信手机端扫码，在微信手机端App内置的浏览器中打开网页
* 网页要求用户身份，指示用户到AlphaID完成身份验证
* 用户没有其他外部登录会话

微信提供了[微信开发者工具](https://developers.weixin.qq.com/miniprogram/dev/devtools/download.html)，用于在PC上模拟微信内置浏览器环境。[微信公众平台](https://mp.weixin.qq.com/)为公众号提供了测试号，可用于在开发阶段调试公众号相关接口功能。

尽管微信提供了上述工具和测试号信息，但开发阶段仍存在一些限制：网页授权回调时不支持localhost名称和非标端口。您需要一些技巧来绕过这些限制。

我们建议采用如下方案：

- 假设一个不存在的域名，例如：`alpha-id.wechat-test.org`
- 在本机搭建一个反向代理，可以使用[ngnix](https://nginx.org/en/)、IIS或其他您熟悉的软件。
- 配置反向代理，使得到假想域名的HTTPS 443标准端口的访问，路由到AuthCenterWebApp调试程序实例
- 修改本地hosts，将假想域名解析到本机（127.0.0.1）
- 启动调试，在微信开发者工具的内置浏览器环境中，用假想域名访问以触发相关验证逻辑。

> 集成测试时，创建HttpClient并替换请求头中的UserAgent字段，使其包含`MicroMessenger`字样，以完成测试。

## 构建

在sln所在目录运行如下命令以进行构建：

``` powershell
dotnet build -c
```

你可以在 Visual Studio 中使用“生成解决方案”的方式构建项目。

## 测试

### 单元测试

使用[xUnit](https://xunit.net/)。

请确保为您的更改添加或调整单元测试，单元测试的设计应始终保持简单语义，并且您应保证完全通过单元测试。

### 集成测试

集成测试**不需要预先准备数据库**，也不需要先运行 `DatabaseTool`：测试会自己建库、自己应用迁移、自己灌数据，
并在进程退出时删除该库。因此每次运行都从同一个已知状态出发，具备回归性。

实现位于 `IntegrationTestUtilities` 项目的 `TestDatabase`，由 `TestDatabase.For(测试程序集)` 取得当前测试项目的实例。

* **库名固定**而非随机：`AlphaIdTest-{测试程序集}-net{主版本}`，例如 `AlphaIdTest-AuthCenterWebApp_Tests-net10`。
  重复运行会重建同一个库，既不积累垃圾库，也不会残留上一次运行的脏数据。
* **建库流程**：删除同名库 → 按各 `DbContext` 应用迁移 → 补齐内置数据并做一次往返校验 → 灌入[示例数据](SampleData.md)。
* **迁移工程**：迁移已抽到类库 `AlphaId.Migrations`，测试直接引用它，不依赖 `DatabaseTool`。
* **环境**：测试仍以 `Development` 环境启动，以复用开发期的模拟外部服务（不实际发信/发短信、验证码总是通过、不实际执行 OCR）；
  但连接字符串被整体重定向到自建库，**不会读写 `appsettings.Development.json` 中的开发库**。

两个可选环境变量：

| 环境变量 | 作用 |
|---|---|
| `ALPHAID_TEST_CONNECTION_STRING` | 覆盖测试库所在的数据库服务器，默认使用本机 LocalDB。连接串中的 `{Database}` 是库名占位符。 |
| `ALPHAID_TEST_KEEP_DATABASE` | 设为非空时，进程退出**不删除**测试库，便于事后排查；下次运行仍会先删除同名库再重建。 |

> 同一项目的多个测试进程（例如并行启动同一测试程序集两次）会竞争同一个库，请避免。不同测试项目因库名含程序集名而互不干扰。

> 在本机直接执行 `dotnet test` 可能报告「运行了零个测试」（退出代码 5），这是 .NET 10 SDK 配合 Microsoft.Testing.Platform 的环境问题，
> 与集成测试的设计无关（未改动的测试项目同样如此）。请直接运行生成的测试可执行文件，例如
> `src/AlphaID/AuthCenterWebApp.Tests/bin/Debug/net10.0/AuthCenterWebApp.Tests.exe`，或为 `dotnet test` 显式指定 `--framework net10.0`。

## 打包和发布

## 贡献

我们热忱期待你的贡献。如果你有意为该项目作贡献，请与作者联系。
