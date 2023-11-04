# Alpha ID 发行说明

Alpha ID 尚未正式发布，目前已实现的特性如下。在正式发布时，特性可能会有所调整。

## 特性

### 主体 Subjects

* 自然人、组织以及组织成员身份
* 针对自然人和组织的自助服务
  * Invite person join in organization
* 组织成员身份的可见性
* Provided WebAPI for Searching person and organization.
* Support Real-Name validation.

### 身份 Identity

* 完整支持 OAuth/OpenID Connect 有关协议
  * 支持 OIDC 的管理
  * 支持颁发所有 OpenID Connect 预定的声明类型
  * 依赖方可以通过 acr-values 指示 Alpha ID 使用指定的外部 IdP

### 账户管理 Account Management

* 可以管理下游 LDAP(Microsoft Active Directory) 基础结构
* 可以接受上游 LDAP(Microsoft Active Directory) 基础结构的管理

### 验证 Authentication

* 使用密码本地登录
  * 可以移除密码以禁用本地登录
* 支持外部登录
* 支持多因子身份验证
  * 支持通过TOPT验证器实施第二因子身份验证
* 对登录失败计数并实施锁定
* 支持密码有效期和强制更改
* Supports binding an exists account after external login.

### 安全性 Security

* 注册阶段实施 CHAPCHA 检查

### 外观 Appearances

* 多语言支持，(en-US and zh-CN).
* 友好 URL (in Auth Center Web Application).

### 系统 System

* 对等多实例结构（启用共享服务端会话时），意味着对NLB友好。