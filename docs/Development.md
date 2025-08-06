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

集成测试时，使用Development环境，与开发调试环境一致，使用[示例数据](SampleData.md)。

## 打包和发布

## 贡献

我们热忱期待你的贡献。如果你有意为该项目作贡献，请与作者联系。
