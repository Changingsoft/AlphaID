# Alpha ID 部署指南

## 需求

### 您组织的IT基础设施

在部署该系统前，如果您的组织通常具有这些IT基础设施服务，则可以支持某些功能特性，或提供更加优秀的应用体验。

* 电子邮件系统

如果组织具有自己的电子邮件系统，则可以借助电子邮件系统来实施用户邮件真实性验证、邮件通知消息、通过邮件重设密码等功能。Alpha ID 使用标准的 SMTP 协议来处理邮件发送。

* 短信发送服务

组织具有从运营商向移动电话号码发送短信的服务，则可以利用短信完成移动电话号码验证、找回密码、多因子登录验证。

> 由于没有标准化的短信接口规范，组织需要自己实现短信发送服务。请参阅。

### 现有标识基础设施

如果您的组织现存有标识基础设施（例如 Active Directory），则 Alpha ID 的设计目标是替代并形成更通用的基础设施。这意味着 Active Directory 可以被 Alpha ID 纳管，但目前还存在一些限制：

* Alpha ID 尚未支持 RADIUS 协议，您仍需要依托 Active Directory 和 NAPS 组件来提供 RADIUS 服务。
* Alpha ID 尚未支持 Kerberos 协议，您仍需要依托 Active Directory 或其他类似组件来提供 Kerberos 协议服务。
* Windows 体系下基于 Kerberos 或 NTLM 协议的应用无法替代，由于设计的原因，这些 Windows 体系的基础设施采用了部分私有实现，无法在标准框架下成功实施。
* 如果您计划纳管 Active Directory，则 Alpha ID 需要部署在 Windows Server 平台上，原因是 Directory Services 组件目前仅在 Windows 平台上支持，我们正在寻求其他跨平台的替代方案。

## 预配置

配置 appsettings.json

## 初始运行

运行 DatabaseTool 工具以准备初始运行

## 升级

如果您组织已存在 Alpha ID 旧版本，则执行升级部署。
运行 DatabaseTool 以准备升级。

**执行升级可能具有一定风险，在开始升级前请务必进行详细评估并备份数据。**

## 多实例和负载均衡

Alpha ID 支持多实例部署。

运行实例所需资源有差异时，应在负载均衡器上调整分发比例，以避免遇到性能瓶颈。

HTTP代理或负载均衡器可以使用根目录下的 /HeartBeat，检测实例是否处于工作状态。

## 限制

### 与Active Directory结合使用时的限制

当Alpha ID计划与Microsoft Active Directory结合使用时，下列系统组件需要部署在加入域环境的Windows Server服务器上，因为操作Active Directory的部分组件目前不支持其他操作系统。
