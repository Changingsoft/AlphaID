# Development Guide

## Requirements

开发环境：

* .NET SDK
* Java Development Kit
* Nodejs



## Debuging

本地开发调试时，Alpha ID不依赖实际的外部服务，它使用模拟的外部服务和本地持久化方案。

* 使用SQL Server Local DB提供持久化。
* 不实际发送邮件，但会在日志中记录消息性日志。
* 不实际发送短信，但会在日志中记录消息性日志。
* 不实际发送短信验证码，但会在日志中记录消息性日志，对验证码的验证总是返回true。

在调试阶段，你可以使用[示例数据](SampleData.md)来模拟完整的应用场景。

## Build

在sln所在目录运行如下命令以进行构建：

``` powershell
dotnet build -c
```
``` bash
rm -rd base/d
```
## Testing

集成测试时，使用Development环境，与开发调试环境一致，使用[示例数据](SampleData.md)。

## Packaging & Publish

## Contribution