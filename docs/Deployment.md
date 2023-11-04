# Alpha ID 部署指南

## 需求

### Your organization IT infrastructures

* 电子邮件系统

An email system or email service will be used for email address validation, reset password, etc. Alpha ID uses SMTP protocol for mail sending.

* 短信接口

Short message service will be used for phone number verification, reset password, receive notice, etc.

## 预配置

配置 appsettings.json

## 初始运行

运行 DatabaseTool 工具以准备初始运行

## 升级

运行 DatabaseTool 以准备升级


## 多实例和负载均衡

Alpha ID 支持多实例部署。

运行实例所需资源有差异时，应在负载均衡器上调整分发比例，以避免遇到性能瓶颈。

HTTP代理或负载均衡器可以使用根目录下的 /Heart，检测实例是否处于工作状态。

## Limitations

### 与Active Directory结合使用时的限制

当Alpha ID计划与Microsoft Active Directory结合使用时，下列系统组件需要部署在加入域环境的Windows Server服务器上，因为操作Active Directory的部分组件目前不支持其他操作系统。